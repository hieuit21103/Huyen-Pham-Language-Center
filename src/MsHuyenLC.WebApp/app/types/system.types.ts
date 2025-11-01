/**
 * Interface cho request tạo cấu hình
 */
export interface CauHinhHeThongRequest {
    ten: string;
    giaTri: string;
    moTa?: string;
}

/**
 * Interface cho request cập nhật cấu hình
 */
export interface CauHinhHeThongUpdateRequest {
    giaTri?: string;
    moTa?: string;
}

/**
 * Interface cho cấu hình hệ thống
 */
export interface CauHinhHeThong {
    id: string;
    ten: string;
    giaTri: string;
    moTa?: string;
    createdAt?: string;
    updatedAt?: string;
}