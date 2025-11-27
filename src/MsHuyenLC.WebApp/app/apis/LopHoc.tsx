import { HocVienApiUrl, LopHocApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    LopHocRequest, 
    LopHocUpdateRequest,
    ApiResponse 
} from "~/types/index";export async function createLopHoc(request: LopHocRequest): Promise<ApiResponse> {
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
 * Lấy danh sách lớp học
 */
export async function getLopHocs(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(LopHocApiUrl(), {
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
        const response = await fetch(HocVienApiUrl(`class/${id}`), {
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
