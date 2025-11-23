import { getJwtToken } from "./Auth";
import { BackupApiUrl } from "~/constants/apis-url";
import type { ApiResponse } from "~/types";


export async function getAllBackups(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl(), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function getBackupById(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl(id), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function getLatestBackup(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl('latest'), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function getBackupCount(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl('count'), {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function createBackup(): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl("create"), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function restoreBackup(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl(`${id}/restore`), {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}

export async function deleteBackup(id: string): Promise<ApiResponse> {
    try {
        const token = getJwtToken();
        const response = await fetch(BackupApiUrl(id), {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` }),
            },
        });

        return await response.json();
    } catch (error: any) {
        return {
            success: false,
            message: error.message || "Đã xảy ra lỗi",
        };
    }
}
