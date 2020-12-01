export interface Photo {
    id: string;
    photoUrl: string;
    thumbnailSmallUrl: string;
    thumbnailLargeUrl: string;
    dateTimeTaken: Date;
    latitude?: number;
    longitude?: number;
    fileName: string;
}
