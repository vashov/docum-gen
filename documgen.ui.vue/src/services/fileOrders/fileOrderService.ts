import { FileOrder } from 'services/fileOrders/models/fileOrder';
import { FileOrderGetListRequest } from 'services/fileOrders/requests/fileOrderGetListRequest';
import { FileOrderRequest } from 'services/fileOrders/requests/fileOrderRequest';
import * as apiClient  from '../../utils/apiClient';
import {
    ENDPOINT_FILE_ORDER_CREATE,
    ENDPOINT_FILE_ORDER_DELETE_FILES,
    ENDPOINT_FILE_ORDER_DOWNLOAD_FILE_RESULT,
    ENDPOINT_FILE_ORDER_LIST
} from '../../configs/config';
import { PageResult } from 'services/fileOrders/models/pageResult';

export async function getFileOrderList (pageQuery: FileOrderGetListRequest): Promise<PageResult<FileOrder> | null> {
    try {
        const response = await apiClient.get<PageResult<FileOrder>, FileOrderGetListRequest>(ENDPOINT_FILE_ORDER_LIST, pageQuery);
        return response;
    } catch (error: any) {
        throw new Error(`Error fetching data: ${error.message}`);
    }
}

export const deleteFilesFromOrder = async (fileOrderId: string): Promise<boolean | null> => {
    try {
        const requestData: FileOrderRequest = {
            fileOrderId: fileOrderId,
        };
        const response = await apiClient.post<boolean, FileOrderRequest>(ENDPOINT_FILE_ORDER_DELETE_FILES, requestData);
        return response;
    } catch (error: any) {
        throw new Error(`Error fetching data: ${error.message}`);
    }
};

export const downloadFileResult = async (fileOrderId: string): Promise<boolean> => {
    try {
        const requestData: FileOrderRequest = {
            fileOrderId: fileOrderId,
        };
        const result = await apiClient.downloadFile<FileOrderRequest>(ENDPOINT_FILE_ORDER_DOWNLOAD_FILE_RESULT, requestData);
        return result;
    } catch (error: any) {
        throw new Error(`Error fetching data: ${error.message}`);
    }
};

export const uploadFileSource = async (file: File): Promise<boolean> => {
    try {
        const isOk = await apiClient.uploadFile(ENDPOINT_FILE_ORDER_CREATE, file);
        return isOk;
    } catch (error: any) {
        throw new Error(`Error uploading file: ${error.message}`);
    }
};
