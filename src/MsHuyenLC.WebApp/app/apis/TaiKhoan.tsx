import { TaiKhoanApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    TaiKhoanRequest,
    TaiKhoanUpdateRequest,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

/**
 * Tạo tài khoản mới
 */
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
 * Lấy danh sách tài khoản (có phân trang)
 */
export async function getTaiKhoans(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${TaiKhoanApiUrl()}?${queryParams.toString()}`
            : TaiKhoanApiUrl();

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
 * Tìm kiếm tài khoản
 */
export async function searchTaiKhoan(
    keyword?: string, 
    vaiTro?: number, 
    trangThai?: number,
    params?: PaginationParams
): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (keyword) queryParams.append('keyword', keyword);
        if (vaiTro !== undefined) queryParams.append('vaiTro', vaiTro.toString());
        if (trangThai !== undefined) queryParams.append('trangThai', trangThai.toString());
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());

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
