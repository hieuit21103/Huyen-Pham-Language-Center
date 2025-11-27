import { CauHinhHeThongApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";
import type { CauHinhHeThong, CauHinhHeThongRequest, CauHinhHeThongUpdateRequest } from "~/types/index";

/**
 * Lấy cấu hình theo tên
 */
export async function getCauHinhByName(ten: string): Promise<ApiResponse<CauHinhHeThong>> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHinhHeThongApiUrl(`by-name/${encodeURIComponent(ten)}`), {
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
 * Tạo cấu hình mới
 */
export async function createCauHinh(request: CauHinhHeThongRequest): Promise<ApiResponse<CauHinhHeThong>> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHinhHeThongApiUrl(), {
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
 * Lấy danh sách cấu hình (có phân trang)
 */
export async function getCauHinhs(): Promise<ApiResponse<CauHinhHeThong[]>> {
    try {
        const token = getJwtToken();

        const url = CauHinhHeThongApiUrl();
        const response = await fetch(url, {
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
 * Cập nhật cấu hình
 */
export async function updateCauHinh(id: string, request: CauHinhHeThongUpdateRequest): Promise<ApiResponse<CauHinhHeThong>> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHinhHeThongApiUrl(id), {
            method: "PUT",
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
 * Xóa cấu hình
 */
export async function deleteCauHinh(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHinhHeThongApiUrl(id), {
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
 * Lấy cấu hình theo ID
 */
export async function getCauHinh(id: string): Promise<ApiResponse<CauHinhHeThong>> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHinhHeThongApiUrl(id), {
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
