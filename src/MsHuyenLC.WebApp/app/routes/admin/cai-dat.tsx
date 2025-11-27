import { Settings as SettingsIcon, Save, Mail, Loader2, AlertCircle } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { getCauHinhs, updateCauHinh, createCauHinh } from "~/apis/CauHinhHeThong";
import type { CauHinhHeThong } from "~/types/index";
import { setLightTheme } from "./_layout";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/enums";

export default function AdminSettings() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [configs, setConfigs] = useState<Record<string, CauHinhHeThong>>({});
  const [isAdmin, setIsAdmin] = useState(false);
  
  const [formData, setFormData] = useState({
    tenTrungTam: "",
    slogan: "",
    diaChi: "",
    emailLienHe: "",
    soDienThoai: "",
    hotline: "",
  });

  useEffect(() => {
    setLightTheme();
    checkAdminAccess();
  }, []);

  const checkAdminAccess = async () => {
    const response = await getProfile();
    if (!response.success || !response.data) {
      navigate("/dang-nhap");
      return;
    }
    if (response.data.vaiTro !== VaiTro.Admin) {
      setIsAdmin(false);
      setLoading(false);
      return;
    }
    setIsAdmin(true);
    loadConfigs();
  };

  const loadConfigs = async () => {
    setLoading(true);
    const response = await getCauHinhs();
    
    if (response.success && Array.isArray(response.data)) {
      const configMap: Record<string, CauHinhHeThong> = {};
      response.data.forEach((config: CauHinhHeThong) => {
        configMap[config.ten] = config;
      });
      setConfigs(configMap);
      
      setFormData({
        tenTrungTam: configMap['tenTrungTam']?.giaTri || "",
        slogan: configMap['slogan']?.giaTri || "",
        diaChi: configMap['diaChi']?.giaTri || "",
        emailLienHe: configMap['emailLienHe']?.giaTri || "",
        soDienThoai: configMap['soDienThoai']?.giaTri || "",
        hotline: configMap['hotline']?.giaTri || "",
      });
    }
    setLoading(false);
  };

  const handleSave = async () => {
    setSaving(true);
    
    try {
      const configsToSave = [
        { key: 'tenTrungTam', value: formData.tenTrungTam, moTa: 'Tên trung tâm' },
        { key: 'slogan', value: formData.slogan, moTa: 'Slogan của trung tâm' },
        { key: 'diaChi', value: formData.diaChi, moTa: 'Địa chỉ trung tâm' },
        { key: 'emailLienHe', value: formData.emailLienHe, moTa: 'Email liên hệ' },
        { key: 'soDienThoai', value: formData.soDienThoai, moTa: 'Số điện thoại' },
        { key: 'hotline', value: formData.hotline, moTa: 'Hotline' },
      ];

      for (const config of configsToSave) {
        if (configs[config.key]) {
          // Update existing config
          await updateCauHinh(configs[config.key].id, {
            giaTri: config.value,
            moTa: config.moTa,
          });
        } else {
          // Create new config
          await createCauHinh({
            ten: config.key,
            giaTri: config.value,
            moTa: config.moTa,
          });
        }
      }

      setMessage("Lưu cài đặt thành công!");
      setTimeout(() => setMessage(""), 3000);
      loadConfigs(); // Reload to get updated data
    } catch (error) {
      setMessage("Lỗi khi lưu cài đặt");
      setTimeout(() => setMessage(""), 3000);
    }
    
    setSaving(false);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <Loader2 className="w-12 h-12 text-gray-900 animate-spin mx-auto mb-4" />
          <p className="text-gray-600">Đang kiểm tra quyền truy cập...</p>
        </div>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-8 max-w-md w-full text-center">
          <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <AlertCircle className="w-8 h-8 text-red-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Truy cập bị từ chối</h2>
          <p className="text-gray-600 mb-6">
            Chỉ Quản trị viên mới có quyền truy cập trang này.
          </p>
          <button
            onClick={() => navigate("/admin")}
            className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
          >
            Quay lại Dashboard
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thành công") ? "bg-green-100 border-green-400 text-green-700" : "bg-red-100 border-red-400 text-red-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Cài đặt hệ thống</h1>
        <p className="text-gray-600 mt-1">Quản lý cấu hình hệ thống</p>
      </div>

      {/* Settings Sections */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* General Settings */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div className="flex items-center space-x-3 mb-6">
            <div className="w-10 h-10 bg-gray-900 rounded-lg flex items-center justify-center">
              <SettingsIcon className="w-5 h-5 text-white" />
            </div>
            <h2 className="text-xl font-bold text-gray-900">Cài đặt chung</h2>
          </div>
          
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Tên trung tâm
              </label>
              <input
                type="text"
                value={formData.tenTrungTam}
                onChange={(e) => setFormData({ ...formData, tenTrungTam: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Slogan
              </label>
              <input
                type="text"
                value={formData.slogan}
                onChange={(e) => setFormData({ ...formData, slogan: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Địa chỉ
              </label>
              <textarea
                rows={3}
                value={formData.diaChi}
                onChange={(e) => setFormData({ ...formData, diaChi: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
          </div>
        </div>

        {/* Contact Settings */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div className="flex items-center space-x-3 mb-6">
            <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
              <Mail className="w-5 h-5 text-white" />
            </div>
            <h2 className="text-xl font-bold text-gray-900">Thông tin liên hệ</h2>
          </div>
          
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Email liên hệ
              </label>
              <input
                type="email"
                value={formData.emailLienHe}
                onChange={(e) => setFormData({ ...formData, emailLienHe: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Số điện thoại
              </label>
              <input
                type="tel"
                value={formData.soDienThoai}
                onChange={(e) => setFormData({ ...formData, soDienThoai: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-900 mb-2">
                Hotline
              </label>
              <input
                type="tel"
                value={formData.hotline}
                onChange={(e) => setFormData({ ...formData, hotline: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              />
            </div>
          </div>
        </div>
      </div>

      {/* Save Button */}
      <div className="flex justify-end">
        <button 
          onClick={handleSave}
          disabled={saving}
          className="bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2 disabled:bg-gray-400 disabled:cursor-not-allowed"
        >
          {saving ? (
            <>
              <Loader2 className="w-5 h-5 animate-spin" />
              <span>Đang lưu...</span>
            </>
          ) : (
            <>
              <Save className="w-5 h-5" />
              <span>Lưu thay đổi</span>
            </>
          )}
        </button>
      </div>
    </div>
  );
}
