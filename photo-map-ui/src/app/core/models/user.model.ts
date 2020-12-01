import { ProcessingStatus } from './processing-status.enum';

export interface User {
    id: number;
    name: string;
    yandexDiskAccessToken?: string;
    yandexDiskTokenExpiresOn?: Date;
    yandexDiskStatus?: ProcessingStatus;
    dropboxAccessToken?: string;
    dropboxTokenExpiresOn?: Date;
    dropboxStatus?: ProcessingStatus;
}
