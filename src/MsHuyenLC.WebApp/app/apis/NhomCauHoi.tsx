import { NhomCauHoiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    NhomCauHoiRequest, 
    NhomCauHoiUpdateRequest,
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
 * Lấy danh sách nhóm câu hỏi
 */
export async function getNhomCauHois(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(NhomCauHoiApiUrl(), {
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
