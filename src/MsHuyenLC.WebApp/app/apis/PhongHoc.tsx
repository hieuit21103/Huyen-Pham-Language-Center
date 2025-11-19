import { PhongHocApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    PhongHocRequest,
    ApiResponse 
} from "~/types/index";

/**
 * Tạo phòng học mới
 */
export async function createPhongHoc(request: PhongHocRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhongHocApiUrl(), {
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
 * Cập nhật phòng học
 */
export async function updatePhongHoc(id: string, request: PhongHocRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhongHocApiUrl(id), {
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
 * Xóa phòng học
 */
export async function deletePhongHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhongHocApiUrl(id), {
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
 * Lấy phòng học theo ID
 */
export async function getPhongHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhongHocApiUrl(id), {
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
 * Lấy danh sách phòng học
 */
export async function getPhongHocs(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(PhongHocApiUrl(), {
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
