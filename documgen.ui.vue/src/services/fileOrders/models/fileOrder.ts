import { FileOrderStatus } from 'services/fileOrders/models/fileOrderStatus';

export interface FileOrder {
    fileOrderId: string;
    fileNameSource: string;
    fileNameResult: string;
    status: FileOrderStatus;
}
