import {HttpResponse} from '@angular/common/http';

const getFileNameFromHttpResponse = (response: HttpResponse<Blob>, fallbackFileName: string): string => {
    let filename: string;

    try {
        const contentDisposition = response.headers.get('content-disposition') ?? '';
        const regExp = /filename\*=UTF-8''([\w%\-.]+)(?:; ?|$)/i;
        const findFilename = regExp.exec(contentDisposition);
        filename = findFilename && findFilename.length > 1 ? decodeURIComponent(findFilename[1]) : fallbackFileName;
    } catch {
        filename = fallbackFileName;
    }

    return filename;
};

export const downloadFileFromHttpResponse = (response: HttpResponse<Blob>, fallbackFileName: string, window: Window, document: Document): void => {
    const fileName = getFileNameFromHttpResponse(response, fallbackFileName);
    const binaryData = [];
    binaryData.push(response.body);

    const downloadLink = document.createElement('a');
    const blob = new Blob(binaryData as BlobPart[], {type: 'blob'});
    // @ts-ignore
    downloadLink.href = window.URL.createObjectURL(blob);

    downloadLink.setAttribute('download', fileName);
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
};
