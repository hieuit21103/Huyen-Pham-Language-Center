import { AuthApiUrl } from "~/constants/apis-url";
import type { 
    LoginRequest, 
    ChangePasswordRequest, 
    ResetPasswordRequest, 
    ConfirmResetPasswordRequest,
    ApiResponse 
} from "~/types/index";

function getJwtToken(): string | null {
    const cookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt_token='));
    return cookie ? cookie.split('=')[1] : null;
}

function setJwtToken(token: string, maxAgeSeconds: number = 60 * 60) {
    document.cookie = `jwt_token=${token}; max-age=${maxAgeSeconds};`;
}

function clearJwtToken() {
    document.cookie = "jwt_token=; path=/; max-age=0;";
}

export async function login(request : LoginRequest): Promise<ApiResponse> {
    const response = await fetch(AuthApiUrl('login'), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });
    
    const data = await response.json();

    if (data) {
        const jwtCookieMaxAgeSeconds = 60 * 60;
        document.cookie = `jwt_token=${data.token}; max-age=${jwtCookieMaxAgeSeconds};`;
    }

    return data;
}

export async function logout() {
    const response = await fetch(AuthApiUrl('logout'), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
    });

    clearJwtToken();
    return { success: true, message: "Đăng xuất thành công" };
}

export async function changePassword(request: ChangePasswordRequest): Promise<ApiResponse> {
    if (document.cookie.indexOf('jwt_token=') === -1) {
        return { success: false, message: "Người dùng chưa đăng nhập" };
    }

    const token = document.cookie
        .split('; ')
        .find(row => row.startsWith('jwt_token='))
        ?.split('=')[1];
        
    const response = await fetch(AuthApiUrl('change-password'), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(request),
    });

    var data = await response.json();
    return data;
}

export async function sendResetPasswordEmail(request: ResetPasswordRequest): Promise<ApiResponse> {
    const response = await fetch(AuthApiUrl('reset-password'), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });

    var data = await response.json();
    return data;
}

export async function confirmResetPassword(request: ConfirmResetPasswordRequest): Promise<ApiResponse> {
    const response = await fetch(AuthApiUrl('reset-password/confirm'), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });

    var data = await response.json();
    return data;
}

export { getJwtToken, setJwtToken, clearJwtToken };

