import { YandexDiskStatus } from './yandex-disk-status.enum';

export class User {
    id: number;
    name: string;
    yandexDiskAccessToken: string;
    yandexDiskTokenExpiresOn: Date;
    yandexDiskStatus: YandexDiskStatus;
}
