import { DeThiApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type {
    DeThiRequest,
    DeThiUpdateRequest,
    GenerateTestRequest,
    GenerateTestWithDifficultyRequest,
    PaginationParams,
    ApiResponse
} from "~/types/index";

/**
 * Tự động tạo đề thi với câu hỏi ngẫu nhiên
 */
export async function generateDeThi(request: GenerateTestRequest): Promise<ApiResponse> {
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
 * Tạo đề thi với phân bổ câu hỏi theo độ khó
 */
export async function generateDeThiWithDifficulty(request: GenerateTestWithDifficultyRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl('generate-with-difficulty'), {
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
 * Tạo đề thi hỗn hợp với cả nhóm câu hỏi và câu hỏi độc lập
 */
export async function createMixedDeThi(request: {
    tenDe: string;
    thoiGianLamBai: number;
    loaiDeThi: number;
    kyThiId?: string;
    nhomCauHoiIds: string[];
    cauHoiDocLapIds: string[];
}): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl('create-mixed'), {
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
 * Lấy danh sách đề thi với phân trang
 */
export async function getDeThis(params?: PaginationParams): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const queryParams = new URLSearchParams();
        if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
        if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params?.sortBy) queryParams.append('sortBy', params.sortBy);
        if (params?.sortOrder) queryParams.append('sortOrder', params.sortOrder);

        const url = queryParams.toString() 
            ? `${DeThiApiUrl()}?${queryParams.toString()}`
            : DeThiApiUrl();

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
 * Lấy câu hỏi của đề thi (flat list - deprecated)
 */
export async function getDeThiQuestions(deThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(`${deThiId}/questions`), {
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

/**
 * Xuất đề thi ra file Word
 */
export async function exportDeThiToWord(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(`${id}/export/word`), {
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

/**
 * Lấy danh sách đề thi theo kỳ thi
 */
export async function getDeThiByKyThi(kyThiId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(DeThiApiUrl(`kythi/${kyThiId}`), {
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
