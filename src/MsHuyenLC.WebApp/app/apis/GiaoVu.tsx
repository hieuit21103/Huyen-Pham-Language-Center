import { GiaoVuApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { ApiResponse } from "~/types/index";
import type { GiaoVu, GiaoVuRequest } from "~/types/index";

/**
 * Lấy giáo vụ theo tài khoản ID
 */
export async function getGiaoVuByTaiKhoanId(taiKhoanId: string): Promise<ApiResponse<GiaoVu>> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(`taikhoan/${taiKhoanId}`), {
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
 * Tạo giáo vụ mới
 */
export async function createGiaoVu(request: GiaoVuRequest): Promise<ApiResponse<GiaoVu>> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(), {
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
 * Lấy danh sách giáo vụ
 */
export async function getGiaoVus(): Promise<ApiResponse<GiaoVu[]>> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(), {
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
 * Cập nhật giáo vụ
 */
export async function updateGiaoVu(id: string, request: GiaoVuRequest): Promise<ApiResponse<GiaoVu>> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(id), {
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
 * Xóa giáo vụ
 */
export async function deleteGiaoVu(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(id), {
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
 * Lấy giáo vụ theo ID
 */
export async function getGiaoVu(id: string): Promise<ApiResponse<GiaoVu>> {
    try {
        const token = getJwtToken();
        const response = await fetch(GiaoVuApiUrl(id), {
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
