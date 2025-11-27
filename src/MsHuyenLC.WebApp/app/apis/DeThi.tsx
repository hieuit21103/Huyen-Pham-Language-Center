import { DeThiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type {
    DeThiRequest,
    DeThiUpdateRequest,
    GenerateExamRequest,
    ApiResponse
} from "~/types/index";

/**
 * Tự động tạo đề thi dựa trên cấu hình kỳ thi (NEW)
 * Sử dụng CauHinhKyThi để random câu hỏi theo CapDo, DoKho, KyNang, CheDoCauHoi
 */
export async function generateExam(request: GenerateExamRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl('generate'), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        const data = await response.json();
        return data;
    } catch (error: ApiResponse | any) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Tạo đề luyện tập tự động với tiêu chí cụ thể
 */
export async function generatePracticeTest(request: {
    capDo: number;
    doKho: number;
    kyNang: number;
    soCauHoi: number;
    thoiLuongPhut: number;
    cheDoCauHoi: number;
}): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl('generate-practice'), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        const data = await response.json();
        return data;
    } catch (error: ApiResponse | any) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Tạo đề thi thủ công với danh sách câu hỏi được chọn
 */
export async function createDeThi(request: DeThiRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        const data = await response.json();
        return data;
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Cập nhật đề thi
 */
export async function updateDeThi(id: string, request: DeThiUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(id), {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
            body: JSON.stringify(request),
        });

        const data = await response.json();
        return data;
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Xóa đề thi
 */
export async function deleteDeThi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(id), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        var data = await response.json();
        return data;
    } catch (error) {
        return { success: false, message: `Lỗi: ${error}` };
    }
}

/**
 * Lấy đề thi theo ID
 */
export async function getDeThi(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(id), {
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
 * Lấy danh sách đề thi
 */
export async function getDeThis(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(), {
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
 * Lấy câu hỏi của đề thi theo nhóm (RECOMMENDED)
 */
export async function getDeThiQuestionsGrouped(deThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(`${deThiId}/questions-grouped`), {
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


