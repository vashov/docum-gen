export enum FileOrderStatus {
    Created = 1,
    Failed = 2,
    Processed = 3,
    FilesDeleted = 4
}

export function getStatusDescription (status: FileOrderStatus): string {
    switch (status) {
        case FileOrderStatus.Created:
            return 'Created';
        case FileOrderStatus.Failed:
            return 'Failed';
        case FileOrderStatus.Processed:
            return 'Processed';
        case FileOrderStatus.FilesDeleted:
            return 'Files deleted';
        default:
            return 'Unknown status';
    }
}
