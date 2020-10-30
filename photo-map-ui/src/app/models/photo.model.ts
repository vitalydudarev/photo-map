export interface Photo {
    id: string;
    photoUrl: string;
    thumbnailSmallFileId: number;
    thumbnailLargeFileId: number;
    dateTimeTaken: Date;
    latitude?: number;
    longitude?: number;
    fileName: string;
    thumbnailUrl: string;
}
