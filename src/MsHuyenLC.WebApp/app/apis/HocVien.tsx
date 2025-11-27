import { HocVienApiUrl } from "~/constants/apis-url";
import { getJwtToken } from "./Auth";
import type { 
    HocVienUpdateRequest,
    ApiResponse 
} from "~/types/index";

export async function updateHocVien(id: string, request: HocVienUpdateRequest): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(HocVienApiUrl(id), {
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

export async function deleteHocVien(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(HocVienApiUrl(id), {
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

export async function getHocVien(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(HocVienApiUrl(id), {
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

export async function getByTaiKhoanId(taiKhoanId: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(HocVienApiUrl(`taikhoan/${taiKhoanId}`), {
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

export async function getHocViens(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(HocVienApiUrl(), {
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
