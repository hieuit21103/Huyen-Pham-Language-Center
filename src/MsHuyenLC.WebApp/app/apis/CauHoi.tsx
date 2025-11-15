import { CauHoiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    CauHoiRequest, 
    CauHoiUpdateRequest,
    CauHoiFilterParams,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

/**
 * Tạo câu hỏi mới
 */
export async function createCauHoi(request: CauHoiRequest): Promise<ApiResponse> {
    try {
        const array = [request];
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl(), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(array),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Cập nhật câu hỏi
 */
export async function updateCauHoi(id: string, request: CauHoiUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl(id), {
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
 * Xóa câu hỏi
 */
export async function deleteCauHoi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl(id), {
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
 * Lấy câu hỏi theo ID
 */
export async function getCauHoi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl(id), {
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
 * Lấy danh sách câu hỏi (có phân trang)
 */
export async function getCauHois(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${CauHoiApiUrl()}?${queryParams.toString()}`
            : CauHoiApiUrl();

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
 * Tìm kiếm câu hỏi theo bộ lọc
 */
export async function searchCauHois(filters: CauHoiFilterParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (filters.pageNumber) queryParams.append('pageNumber', filters.pageNumber.toString());
        if (filters.pageSize) queryParams.append('pageSize', filters.pageSize.toString());
        if (filters.sortBy) queryParams.append('sortBy', filters.sortBy);
        if (filters.sortOrder) queryParams.append('sortOrder', filters.sortOrder);
        if (filters.loaiCauHoi !== undefined) queryParams.append('loaiCauHoi', filters.loaiCauHoi.toString());
        if (filters.kyNang !== undefined) queryParams.append('kyNang', filters.kyNang.toString());
        if (filters.capDo !== undefined) queryParams.append('capDo', filters.capDo.toString());
        if (filters.doKho !== undefined) queryParams.append('doKho', filters.doKho.toString());
        if (filters.keyword) queryParams.append('keyword', filters.keyword);

        const url = `${CauHoiApiUrl('search')}?${queryParams.toString()}`;

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
 * Import câu hỏi từ file Excel
 */
export async function importCauHoisFromExcel(file: File): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const formData = new FormData();
        formData.append('file', file);

        const response = await fetch(CauHoiApiUrl('import'), {
            method: "POST",
            headers: {
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: formData,
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xuất template Excel để import câu hỏi
 */
export async function downloadImportTemplate(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl('download-template'), {
            method: "GET",
            headers: {
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        const blob = await response.blob();
        return { success: true, data: blob };
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa nhiều câu hỏi cùng lúc
 */
export async function deleteBulkCauHois(ids: string[]): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(CauHoiApiUrl('bulk-delete'), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(ids),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}
