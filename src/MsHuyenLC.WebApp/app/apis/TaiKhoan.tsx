import { TaiKhoanApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    TaiKhoanRequest, 
    TaiKhoanUpdateRequest,
    ApiResponse 
} from "~/types/index";

export async function createTaiKhoan(request: TaiKhoanRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(TaiKhoanApiUrl(), {
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
 * Cập nhật tài khoản
 */
export async function updateTaiKhoan(id: string, request: TaiKhoanUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(TaiKhoanApiUrl(id), {
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
 * Xóa tài khoản
 */
export async function deleteTaiKhoan(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(TaiKhoanApiUrl(id), {
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
 * Lấy danh sách tài khoản
 */
export async function getTaiKhoans(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(TaiKhoanApiUrl(), {
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
 * Tìm kiếm tài khoản
 */
export async function searchTaiKhoans(
    keyword?: string, 
    vaiTro?: number, 
    trangThai?: number
): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (keyword) queryParams.append('keyword', keyword);
        if (vaiTro !== undefined) queryParams.append('vaiTro', vaiTro.toString());
        if (trangThai !== undefined) queryParams.append('trangThai', trangThai.toString());

        const url = queryParams.toString() 
            ? `${TaiKhoanApiUrl('search')}?${queryParams.toString()}`
            : TaiKhoanApiUrl('search');

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
