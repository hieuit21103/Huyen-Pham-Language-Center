import { PhanCongApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    PhanCongRequest,
    ApiResponse 
} from "~/types/index";

/**
 * Tạo phân công giảng dạy mới
 */
export async function createPhanCong(request: PhanCongRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa phân công
 */
export async function deletePhanCong(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(id), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy phân công theo ID
 */
export async function getPhanCong(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(id), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách phân công
 */
export async function getPhanCongs(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách phân công theo giáo viên
 */
export async function getPhanCongByGiaoVien(giaoVienId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(`giaovien/${giaoVienId}`), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy danh sách phân công theo lớp học
 */
export async function getPhanCongByLopHoc(lopHocId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhanCongApiUrl(`lophoc/${lopHocId}`), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}
