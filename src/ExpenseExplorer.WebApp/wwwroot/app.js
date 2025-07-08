
window.downloadFile = (fileName, contentType, content) => {
    const link = document.createElement('a');
    const file = new Blob([content], { type: contentType });
    link.href = URL.createObjectURL(file);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href);
};
