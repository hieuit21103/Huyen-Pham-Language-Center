import { BASE_API_URL } from "~/constants/apis-url";
import { getJwtToken } from "~/apis/Auth";

export async function uploadImage(file: File): Promise<{ success: boolean; url?: string; message: string }> {
  try {
    const token = getJwtToken();
    if (!token) {
      return { success: false, message: "Người dùng chưa đăng nhập" };
    }

    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${BASE_API_URL}/upload/image`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: 'Lỗi khi upload hình ảnh' };
  }
}

export async function uploadAudio(file: File): Promise<{ success: boolean; url?: string; message: string }> {
  try {

    const token = getJwtToken();
    if (!token) {
      return { success: false, message: "Người dùng chưa đăng nhập" };
    }

    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${BASE_API_URL}/upload/audio`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: 'Lỗi khi upload âm thanh' };
  }
}

export async function uploadAvatar(file: File): Promise<{ success: boolean; url?: string; message: string }> {
  try {
    const token = getJwtToken();
    if (!token) {
      return { success: false, message: "Người dùng chưa đăng nhập" };
    }
    const formData = new FormData();
    formData.append('file', file);
    const response = await fetch(`${BASE_API_URL}/upload/avatar`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    return await response.json();
  } catch (error) {
    return { success: false, message: 'Lỗi khi upload avatar' };
  }
}

