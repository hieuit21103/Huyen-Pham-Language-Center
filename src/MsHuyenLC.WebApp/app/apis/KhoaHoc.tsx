import { KhoaHocApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    KhoaHocRequest,
    KhoaHocUpdateRequest,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

/**
 * Tạo khóa học mới
 */
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
 * Lấy danh sách khóa học (có phân trang)
 */
export async function getKhoaHocs(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${KhoaHocApiUrl()}?${queryParams.toString()}`
            : KhoaHocApiUrl();

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
