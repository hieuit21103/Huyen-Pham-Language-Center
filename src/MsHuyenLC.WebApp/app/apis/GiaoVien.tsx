import { GiaoVienApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    GiaoVienRequest, 
    GiaoVienUpdateRequest,
    ApiResponse 
} from "~/types/index";/**
 * Tạo giáo viên mới
 */
export async function createGiaoVien(request: GiaoVienRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVienApiUrl(), {
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
 * Cập nhật giáo viên
 */
export async function updateGiaoVien(id: string, request: GiaoVienUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVienApiUrl(id), {
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
 * Xóa giáo viên
 */
export async function deleteGiaoVien(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVienApiUrl(id), {
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
 * Lấy giáo viên theo ID
 */
export async function getGiaoVien(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVienApiUrl(id), {
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
 * Lấy danh sách giáo viên
 */
export async function getGiaoViens(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVienApiUrl(), {
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
