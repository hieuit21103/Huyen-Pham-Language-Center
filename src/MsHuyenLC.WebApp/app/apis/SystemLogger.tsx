import { SystemLoggerApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    PaginationParams,
    ApiResponse 
} from "~/types/index";

/**
 * Lấy log theo ID
 */
export async function getSystemLog(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(SystemLoggerApiUrl(id), {
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
 * Lấy danh sách log (có phân trang)
 */
export async function getSystemLogs(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${SystemLoggerApiUrl()}?${queryParams.toString()}`
            : SystemLoggerApiUrl();

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
 * Lấy log theo tài khoản
 */
export async function getSystemLogsByUser(
    taiKhoanId: string, 
    fromDate?: string, 
    toDate?: string
): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (fromDate) queryParams.append('fromDate', fromDate);
        if (toDate) queryParams.append('toDate', toDate);

        const url = queryParams.toString() 
            ? `${SystemLoggerApiUrl(`by-user/${taiKhoanId}`)}?${queryParams.toString()}`
            : SystemLoggerApiUrl(`by-user/${taiKhoanId}`);

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
 * Lấy log theo khoảng thời gian
 */
export async function getSystemLogsByDateRange(
    fromDate: string, 
    toDate: string,
    params?: PaginationParams
): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        queryParams.append('fromDate', fromDate);
        queryParams.append('toDate', toDate);
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());

        const url = `${SystemLoggerApiUrl('by-date-range')}?${queryParams.toString()}`;

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
 * Tìm kiếm log
 */
export async function searchSystemLogs(
    keyword?: string,
    action?: string,
    params?: PaginationParams
): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (keyword) queryParams.append('keyword', keyword);
        if (action) queryParams.append('action', action);
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());

        const url = queryParams.toString() 
            ? `${SystemLoggerApiUrl('search')}?${queryParams.toString()}`
            : SystemLoggerApiUrl('search');

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
