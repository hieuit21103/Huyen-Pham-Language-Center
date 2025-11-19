import { KhoaHocApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    KhoaHocRequest, 
    KhoaHocUpdateRequest,
    ApiResponse 
} from "~/types/index";

export async function createKhoaHoc(request: KhoaHocRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KhoaHocApiUrl(), {
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
 * Cập nhật khóa học
 */
export async function updateKhoaHoc(id: string, request: KhoaHocUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KhoaHocApiUrl(id), {
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
 * Xóa khóa học
 */
export async function deleteKhoaHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KhoaHocApiUrl(id), {
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
 * Lấy khóa học theo ID
 */
export async function getKhoaHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KhoaHocApiUrl(id), {
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
 * Lấy danh sách khóa học
 */
export async function getKhoaHocs(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(KhoaHocApiUrl(), {
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
