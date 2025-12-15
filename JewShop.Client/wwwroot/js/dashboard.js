(() => {
    const textEncoder = new TextEncoder();

    const sanitizeLine = (value) =>
        (value ?? '')
            .normalize('NFD')
            .replace(/[^\u0009\u000A\u000D\u0020-\u007E]/g, '')
            .trim();

    const escapePdfText = (value) =>
        (value ?? '')
            .replace(/\\/g, '\\\\')
            .replace(/\(/g, '\\(')
            .replace(/\)/g, '\\)');

    const buildContentStream = (lines) => {
        const content = [];
        let y = 780;
        for (const line of lines) {
            const safeLine = sanitizeLine(line);
            content.push(`BT /F1 12 Tf 50 ${y} Td (${escapePdfText(safeLine)}) Tj ET`);
            y -= 18;
            if (y < 60) {
                content.push(`BT /F1 12 Tf 50 ${y} Td (Vui long thu gon noi dung) Tj ET`);
                break;
            }
        }
        const stream = content.join('\n');
        return {
            stream,
            length: textEncoder.encode(stream).length
        };
    };

    const createSimplePdf = (lines) => {
        const objects = [];
        const addObject = (body) => {
            const index = objects.length + 1;
            objects.push(`${index} 0 obj\n${body}\nendobj\n`);
            return index;
        };

        const { stream, length } = buildContentStream(lines);
        const contentRef = addObject(`<< /Length ${length} >>\nstream\n${stream}\nendstream`);
        const fontRef = addObject('<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>');
        const pageRef = addObject(`<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents ${contentRef} 0 R /Resources << /Font << /F1 ${fontRef} 0 R >> >> >>`);
        const pagesRef = addObject(`<< /Type /Pages /Kids [${pageRef} 0 R] /Count 1 >>`);
        const catalogRef = addObject(`<< /Type /Catalog /Pages ${pagesRef} 0 R >>`);

        let pdf = '%PDF-1.4\n';
        const offsets = [0];
        for (const obj of objects) {
            offsets.push(pdf.length);
            pdf += obj;
        }

        const xrefPosition = pdf.length;
        pdf += `xref\n0 ${objects.length + 1}\n0000000000 65535 f \n`;
        for (let i = 1; i <= objects.length; i++) {
            pdf += `${offsets[i].toString().padStart(10, '0')} 00000 n \n`;
        }

        pdf += `trailer\n<< /Size ${objects.length + 1} /Root ${catalogRef} 0 R >>\nstartxref\n${xrefPosition}\n%%EOF`;
        return pdf;
    };

    const resolveProp = (obj, propName) => {
        if (!obj) {
            return undefined;
        }

        if (Object.prototype.hasOwnProperty.call(obj, propName)) {
            return obj[propName];
        }

        const camel = propName.charAt(0).toLowerCase() + propName.slice(1);
        if (Object.prototype.hasOwnProperty.call(obj, camel)) {
            return obj[camel];
        }

        return obj[propName] ?? obj[camel];
    };

    const buildLines = (payload) => {
        if (!payload) {
            return [];
        }

        const lines = [];
        const pick = (name, fallback = '-') => resolveProp(payload, name) ?? fallback;

        lines.push('BAO CAO TRUNG TAM THONG KE');
        lines.push(`Thoi gian: ${pick('GeneratedAt')}`);
        lines.push('--- Tong quan ---');
        lines.push(`San pham: ${pick('ProductCount')}`);
        lines.push(`Nha cung cap: ${pick('SupplierCount')}`);
        lines.push(`Gia tri ton kho: ${pick('TotalInventoryValue')}`);
        lines.push(`Gia trung binh: ${pick('AverageProductPrice')}`);
        lines.push('--- Ma giam gia ---');
        lines.push(`Tong ma: ${pick('CouponCount')}`);
        lines.push(`Hoat dong: ${pick('ActiveCouponCount')}`);
        lines.push(`Sap mo: ${pick('UpcomingCouponCount')}`);
        lines.push(`Het han: ${pick('ExpiredCouponCount')}`);
        lines.push('--- Canh bao ton kho ---');
        lines.push(`Het hang: ${pick('OutOfStockCount')}`);
        const threshold = pick('LowStockThreshold');
        lines.push(`Can nhap (<= ${threshold}): ${pick('LowStockCount')}`);
        lines.push('--- SKU can xu ly ---');

        const products = resolveProp(payload, 'LowStockProducts') || [];
        if (products.length) {
            products.forEach((item, index) => {
                const name = resolveProp(item, 'Name') || '-';
                const pid = resolveProp(item, 'ProductId') || '-';
                const stock = resolveProp(item, 'StockQuantity') || '-';
                const price = resolveProp(item, 'Price') || '-';
                lines.push(`${index + 1}. #${pid} ${name}`);
                lines.push(`   Ton: ${stock} | Gia: ${price}`);
            });
        } else {
            lines.push('Khong co SKU duoi nguong.');
        }

        return lines;
    };

    window.dashboardDownloadPdf = (fileName, payload) => {
        if (!fileName || !payload) {
            return;
        }

        const lines = buildLines(payload);
        const pdfContent = createSimplePdf(lines);
        const blob = new Blob([pdfContent], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName;
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
        URL.revokeObjectURL(url);
    };
})();
