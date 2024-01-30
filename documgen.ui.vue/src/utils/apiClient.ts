import { API_URL } from '../configs/config';

export const get = async <T, TQueryParams>(endpoint: string, queryParams?: TQueryParams): Promise<T | null> => {
    const url = new URL(`${API_URL}/${endpoint}`);

    try {
        // Append query parameters if provided
        if (queryParams) {
            Object.entries(queryParams).forEach(([key, value]) => {
                url.searchParams.append(key, String(value));
            });
        }

        const response = await fetch(url.toString());
        const data = await response.json();
        return data;
    } catch (error) {
        console.error(`Error get: ${url.toString()}`, error);
        return null;
    }
};

export const post = async <T, TT>(endpoint: string, model: TT): Promise<T | null> => {
    const url = new URL(`${API_URL}/${endpoint}`);

    try {
        const response = await fetch(url.toString(), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(model),
        });

        const contentType = response.headers.get('Content-Type');
        const isJsonResponse = contentType?.includes('application/json');
        if (isJsonResponse)
            return await response.json();
        return null;
    } catch (error) {
        console.error(`Error post: ${url.toString()}`, error);
        return null;
    }
};

export const downloadFile = async <TQueryParams>(endpoint: string, queryParams?: TQueryParams): Promise<boolean> => {
    const url = new URL(`${API_URL}/${endpoint}`);

    try {
        // Append query parameters if provided
        if (queryParams) {
            Object.entries(queryParams).forEach(([key, value]) => {
                url.searchParams.append(key, String(value));
            });
        }

        const response: Response = await fetch(url.toString());
        const blob = await response.blob();
        // Create a download link
        const downloadLink = document.createElement('a');
        downloadLink.href = URL.createObjectURL(blob);
        downloadLink.download = getFileName(response);

        // Append the link to the body, click it, and remove it
        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);
        return true;
    } catch (error) {
        console.error(`Error downloading file: ${url.toString()}`, error);
        return false;
    }
};

export const uploadFile = async (endpoint: string, file: File): Promise<boolean> => {
    const url = new URL(`${API_URL}/${endpoint}`);

    try {
        const formData = new FormData();
        formData.append('file', file);

        const response = await fetch(url.toString(), {
            method: 'POST',
            body: formData,
        });
        return response.ok;
        
    } catch (error) {
        console.error(`Error uploading file: ${url.toString()}`, error);
        return false;
    }
};

const getFileName = (response: Response) => {
    const contentDispositionHeader = response.headers.get('content-disposition');
    if (contentDispositionHeader) {
        const matches = contentDispositionHeader.match(/filename="(.+)"|filename=([^;]+)/);
        if (matches && matches.length > 1) {
            // Use the first non-empty match as the filename
            const filename = matches[1] || matches[2];
            return filename;
        }
    }
    return 'downloaded_file';
}