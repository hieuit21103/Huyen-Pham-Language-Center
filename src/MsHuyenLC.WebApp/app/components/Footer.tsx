import { Phone, Mail, MapPin, Globe, Facebook } from "lucide-react";
import { useState, useEffect } from "react";
import { getCauHinhByName } from "~/apis/CauHinhHeThong";

export default function Footer() {
    const [configs, setConfigs] = useState<Record<string, string>>({
      tenTrungTam: "Huyen Pham Language Center",
      slogan: "Trung tâm ngoại ngữ chuyên nghiệp",
      diaChi: "",
      emailLienHe: "",
      soDienThoai: "",
      hotline: "",
    });

    useEffect(() => {
      loadConfigs();
    }, []);

    const loadConfigs = async () => {
      // Danh sách các config cần lấy
      const configNames = ['tenTrungTam', 'slogan', 'diaChi', 'emailLienHe', 'soDienThoai', 'hotline'];
      
      // Gọi API song song cho tất cả configs
      const responses = await Promise.all(
        configNames.map(name => getCauHinhByName(name))
      );

      const configMap: Record<string, string> = {};
      responses.forEach((response, index) => {
        if (response.success && response.data) {
          configMap[configNames[index]] = response.data.giaTri;
        }
      });
      
      setConfigs({
        tenTrungTam: configMap['tenTrungTam'] || "Huyen Pham Language Center",
        slogan: configMap['slogan'] || "Trung tâm ngoại ngữ chuyên nghiệp",
        diaChi: configMap['diaChi'] || "",
        emailLienHe: configMap['emailLienHe'] || "",
        soDienThoai: configMap['soDienThoai'] || "",
        hotline: configMap['hotline'] || "",
      });
    };

    return (
        <footer className="bg-gray-900 text-white py-12 mt-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            {/* Giới thiệu */}
            <div>
              <h3 className="text-xl font-bold mb-4">GIỚI THIỆU</h3>
              <p className="text-gray-300 text-sm leading-relaxed">
                {configs.tenTrungTam} - {configs.slogan}
              </p>
            </div>

            {/* Địa chỉ */}
            <div>
              <h3 className="text-xl font-bold mb-4">ĐỊA CHỈ</h3>
              <div className="space-y-3 text-gray-300 text-sm">
                <div className="flex items-start">
                  <MapPin className="w-5 h-5 mr-2 flex-shrink-0 mt-1" />
                  <p>
                    {configs.diaChi || "[Địa chỉ sẽ được cập nhật]"}
                  </p>
                </div>
              </div>
            </div>

            {/* Thông tin liên hệ */}
            <div>
              <h3 className="text-xl font-bold mb-4">THÔNG TIN LIÊN HỆ:</h3>
              <div className="space-y-3 text-gray-300 text-sm">
                {configs.hotline && (
                  <div className="flex items-center">
                    <Phone className="w-5 h-5 mr-2 flex-shrink-0" />
                    <span>Hotline: {configs.hotline}</span>
                  </div>
                )}
                {configs.soDienThoai && (
                  <div className="flex items-center">
                    <Phone className="w-5 h-5 mr-2 flex-shrink-0" />
                    <span>Điện thoại: {configs.soDienThoai}</span>
                  </div>
                )}
                {configs.emailLienHe && (
                  <div className="flex items-center">
                    <Mail className="w-5 h-5 mr-2 flex-shrink-0" />
                    <span>Email: {configs.emailLienHe}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Lối tắt */}
            <div>
              <h3 className="text-xl font-bold mb-4">LỐI TẮT</h3>
              <ul className="space-y-2 text-gray-300 text-sm">
                <li>
                  <a href="/" className="hover:text-white transition-colors">Trang chủ</a>
                </li>
                <li>
                  <a href="/khoa-hoc" className="hover:text-white transition-colors">Khóa học</a>
                </li>
                <li>
                  <a href="/luyen-thi" className="hover:text-white transition-colors">Luyện đề</a>
                </li>
              </ul>
              
              {/* Social Icons */}
              <div className="mt-6 flex space-x-4">
                <a href="#" className="text-gray-300 hover:text-white transition-colors">
                  <Mail className="w-6 h-6" />
                </a>
                <a href="#" className="text-gray-300 hover:text-white transition-colors">
                  <Facebook className="w-6 h-6" />
                </a>
              </div>
            </div>
          </div>
        </div>
      </footer>
    );
};