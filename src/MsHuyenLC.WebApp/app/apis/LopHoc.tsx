import { LopHocApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    LopHocRequest,
    LopHocUpdateRequest,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

export async function createLopHoc(request: LopHocRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(), {
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
 * Cập nhật lớp học
 */
export async function updateLopHoc(id: string, request: LopHocUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(id), {
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
 * Xóa lớp học
 */
export async function deleteLopHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(id), {
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
 * Lấy lớp học theo ID
 */
export async function getLopHoc(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(id), {
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
 * Lấy danh sách lớp học (có phân trang)
 */
export async function getLopHocs(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${LopHocApiUrl()}?${queryParams.toString()}`
            : LopHocApiUrl();

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
 * Lấy danh sách học viên trong lớp học
 */
export async function getLopHocStudents(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(`${id}/students`), {
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
