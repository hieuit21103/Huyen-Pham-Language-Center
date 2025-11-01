import { NhomCauHoiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    NhomCauHoiRequest, 
    NhomCauHoiUpdateRequest,
    PaginationParams,
    ApiResponse 
} from "~/types/index";

export async function createNhomCauHoi(request: NhomCauHoiRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(), {
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
 * Cập nhật nhóm câu hỏi
 */
export async function updateNhomCauHoi(id: string, request: NhomCauHoiUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(id), {
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
 * Xóa nhóm câu hỏi
 */
export async function deleteNhomCauHoi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(id), {
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
 * Lấy danh sách nhóm câu hỏi (có phân trang)
 */
export async function getNhomCauHois(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${NhomCauHoiApiUrl()}?${queryParams.toString()}`
            : NhomCauHoiApiUrl();

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
 * Thêm câu hỏi vào nhóm
 */
export async function addCauHoiToNhom(nhomId: string, cauHoiId: string, thuTu?: number): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${nhomId}/add-question`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify({ cauHoiId, thuTu }),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa câu hỏi khỏi nhóm
 */
export async function removeCauHoiFromNhom(nhomId: string, cauHoiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${nhomId}/remove-question/${cauHoiId}`), {
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
 * Lấy danh sách câu hỏi trong nhóm
 */
export async function getCauHoisInNhom(nhomId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${nhomId}/questions`), {
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
 * Sắp xếp lại thứ tự câu hỏi trong nhóm
 */
export async function reorderCauHoisInNhom(nhomId: string, cauHoiOrders: Array<{cauHoiId: string, thuTu: number}>): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${nhomId}/reorder`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify({ cauHoiOrders }),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Sao chép nhóm câu hỏi
 */
export async function cloneNhomCauHoi(id: string, tieuDeMoi?: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${id}/clone`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify({ tieuDeMoi }),
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Tìm kiếm nhóm câu hỏi theo bộ lọc
 */
export async function searchNhomCauHois(filters: {
    capDo?: number;
    doKho?: number;
    keyword?: string;
    pageNumber?: number;
    pageSize?: number;
}): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (filters.pageNumber) queryParams.append('pageNumber', filters.pageNumber.toString());
        if (filters.pageSize) queryParams.append('pageSize', filters.pageSize.toString());
        if (filters.capDo !== undefined) queryParams.append('capDo', filters.capDo.toString());
        if (filters.doKho !== undefined) queryParams.append('doKho', filters.doKho.toString());
        if (filters.keyword) queryParams.append('keyword', filters.keyword);

        const url = `${NhomCauHoiApiUrl('search')}?${queryParams.toString()}`;

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
 * Import nhóm câu hỏi từ file Excel
 */
export async function importNhomCauHoisFromExcel(file: File): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const formData = new FormData();
        formData.append('file', file);

        const response = await fetch(NhomCauHoiApiUrl('import'), {
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
 * Xuất nhóm câu hỏi ra PDF
 */
export async function exportNhomCauHoiToPDF(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(`${id}/export/pdf`), {
            method: "GET",
            headers: {
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}
