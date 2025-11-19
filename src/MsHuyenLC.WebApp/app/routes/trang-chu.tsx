import { useState, useEffect } from 'react';
import { BookOpen, Users, Target, Award, Check } from 'lucide-react';
import { Asset } from '~/assets/Asset';
import type { DangKyKhachRequest } from '~/types/index';
import { registerGuest } from '~/apis/DangKy';
import { getKhoaHocs } from '~/apis';

export default function LanguageCenterLanding() {
  const [showSuccess, setShowSuccess] = useState(false);
  const [loading, setLoading] = useState(true);
  const [courses, setCourses] = useState<any[]>([]);
  const [formData, setFormData] = useState({
    hoTen: '',
    email: '',
    soDienThoai: '',
    gioiTinh: 0,
    khoaHoc: '',
    noiDung: ''
  });

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
      try {
        setLoading(true);
        const response = await getKhoaHocs();
        if (Array.isArray(response.data)) {
          const courses = response.data.map((course: any) => ({
            id: course.id,
            title: course.tenKhoaHoc
          }));
          setCourses(courses);
        }
      } catch (error) {
        console.error('Lỗi khi tải khóa học:', error);
      } finally {
        setLoading(false);
      }
    };

  const features = [
    {
      icon: <Users className="w-12 h-12" />,
      title: 'Giáo Viên Chuyên Nghiệp',
      description: 'Đội ngũ giảng viên giàu kinh nghiệm, tận tâm'
    },
    {
      icon: <BookOpen className="w-12 h-12" />,
      title: 'Chương Trình Chuẩn Quốc Tế',
      description: 'Giáo trình hiện đại, phù hợp mọi trình độ'
    },
    {
      icon: <Target className="w-12 h-12" />,
      title: 'Cam Kết Đầu Ra',
      description: 'Học đến khi đạt mục tiêu mong muốn'
    },
    {
      icon: <Award className="w-12 h-12" />,
      title: 'Học Phí Hợp Lý',
      description: 'Chất lượng cao với mức giá cạnh tranh'
    }
  ];

  const partners = [
    {
      name: 'IDP',
      logo: Asset.idp
    },
    {
      name: 'British Council',
      logo: Asset.britishcouncil
    },
    {
      name: 'Cambridge',
      logo: Asset.cambridge
    },
    {
      name: 'Pearson',
      logo: Asset.pte
    }
  ]

  const handleInputChange = (e: { target: { name: string; value: string; }; }) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = () => {
    if (!formData.hoTen || !formData.email || !formData.soDienThoai) {
      alert('Vui lòng điền đầy đủ thông tin bắt buộc!');
      return;
    }

    const dangKyData: DangKyKhachRequest = {
      hoTen: formData.hoTen,
      email: formData.email,
      soDienThoai: formData.soDienThoai,
      khoaHocId: formData.khoaHoc,
      noiDung: formData.noiDung,
      gioiTinh: formData.gioiTinh
    };

    registerGuest(dangKyData).then((response) => {
      if (response.success) {
        setShowSuccess(true);
        setFormData({
          hoTen: '',
          email: '',
          soDienThoai: '',
          khoaHoc: '',
          noiDung: '',
          gioiTinh: 0
        });

        setTimeout(() => setShowSuccess(false), 5000);
      }else{
        alert('Đăng ký thất bại: ' + response.message);
      }
    });
  };

  return (
    <>
      {/* Hero Section */}
      <section className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-gray-50 text-gray-900 rounded-3xl">
        <div className="w-full">
          <div className="grid md:grid-cols-2 gap-12 items-center max-w-7xl mx-auto">
            <div className="text-center md:text-left">
              <h2 className="text-4xl md:text-6xl font-bold mb-3 leading-tight text-gray-700">
                Chinh Phục Ngoại Ngữ Cùng 
              </h2>
              <h1 className="text-5xl md:text-7xl font-bold mb-6 leading-tight bg-gradient-to-r from-gray-900 via-purple-900 to-blue-900 bg-clip-text text-transparent">
                Huyen Pham Language Center
              </h1>
              <p className="text-2xl md:text-3xl mb-8 text-gray-600">
                Nơi ươm mầm ước mơ toàn cầu của bạn
              </p>
              <div className="flex flex-col sm:flex-row gap-4 justify-center md:justify-start">
                <button
                  onClick={() => {
                    setTimeout(() => {
                      if (typeof window !== 'undefined') {
                        document.getElementById('register')?.scrollIntoView({ behavior: 'smooth' });
                      }
                    }, 100);
                  }}
                  className="bg-gray-900 text-white px-10 py-5 rounded-full text-xl font-bold hover:bg-gray-800 hover:scale-105 transition-all shadow-2xl"
                >
                  Đăng Ký Ngay
                </button>
                <button
                  onClick={() => {
                    window.location.href = '/khoa-hoc';
                  }}
                  className="bg-transparent border-4 border-gray-900 text-gray-900 px-10 py-5 rounded-full text-xl font-bold hover:bg-gray-900 hover:text-white transition-all shadow-2xl"
                >
                  Xem Khóa Học
                </button>
              </div>
              
              {/* Stats */}
              <div className="grid grid-cols-3 gap-6 mt-12">
                <div className="text-center">
                  <div className="text-4xl font-bold mb-2 text-gray-900">5000+</div>
                  <div className="text-gray-600">Học Viên</div>
                </div>
                <div className="text-center">
                  <div className="text-4xl font-bold mb-2 text-gray-900">50+</div>
                  <div className="text-gray-600">Giáo Viên</div>
                </div>
                <div className="text-center">
                  <div className="text-4xl font-bold mb-2 text-gray-900">98%</div>
                  <div className="text-gray-600">Hài Lòng</div>
                </div>
              </div>
            </div>
            
            <div className="flex justify-center items-center">
              <div className="w-full max-w-lg aspect-square bg-gray-100 rounded-3xl flex items-center justify-center border-4 border-gray-200 shadow-2xl hover:scale-105 transition-all">
                <img src={Asset.heroImage} alt="Hero" className="h-full w-full rounded-2xl object-contain" />
              </div>
            </div>
          </div>
          
          {/* Scroll Down Indicator */}
          <div className="text-center mt-16 animate-bounce">
            <div className="inline-block">
              <svg className="w-8 h-8 text-gray-900" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 14l-7 7m0 0l-7-7m7 7V3" />
              </svg>
            </div>
            <p className="text-gray-600 mt-2">Cuộn xuống để khám phá</p>
          </div>
        </div>
      </section>

      {/* Main Content - Full Width */}
      <main className="w-full px-4 sm:px-6 lg:px-8 py-12">
        <div className="space-y-16 max-w-7xl mx-auto">
          {/* Promo Banner */}
          <div className="bg-gray-100 rounded-3xl p-8 shadow-2xl border-2 border-gray-200">
            <div className="grid md:grid-cols-2 gap-8 items-center">
              <div className="order-2 md:order-1">
                <div className="aspect-video bg-gray-200 rounded-2xl flex items-center justify-center border-4 border-gray-300">
                  <img src={Asset.promo} alt="Promotional Banner" className="h-full w-full rounded-2xl object-cover" />
                </div>
              </div>
              <div className="order-1 md:order-2 text-center md:text-left">
                <h3 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4">
                  Ưu Đãi Đặc Biệt Tháng Này!
                </h3>
                <p className="text-xl text-gray-700 mb-6">
                  Giảm ngay 20% học phí cho 50 học viên đăng ký đầu tiên
                </p>
                <ul className="text-left space-y-2 text-gray-900 mb-6">
                  <li className="flex items-start">
                    <Check className="w-6 h-6 mr-2 flex-shrink-0" />
                    <span>Tặng thêm 1 tháng học miễn phí</span>
                  </li>
                  <li className="flex items-start">
                    <Check className="w-6 h-6 mr-2 flex-shrink-0" />
                    <span>Miễn phí tài liệu học tập trị giá 500.000đ</span>
                  </li>
                  <li className="flex items-start">
                    <Check className="w-6 h-6 mr-2 flex-shrink-0" />
                    <span>Tặng kèm khóa luyện đề online</span>
                  </li>
                </ul>
                <button
                  onClick={() => {
                    if (typeof window !== 'undefined') {
                      document.getElementById('register')?.scrollIntoView({ behavior: 'smooth' });
                    }
                  }}
                  className="bg-gray-900 text-white px-6 py-3 rounded-full font-bold hover:scale-105 transition-all shadow-xl"
                >
                  Nhận Ưu Đãi Ngay
                </button>
              </div>
            </div>
          </div>

          {/* Features */}
          <div>
            <h2 className="text-4xl font-bold text-center text-gray-900 mb-12">
              Tại Sao Chọn Huyen Pham Language Center?
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
              {features.map((feature, index) => (
                <div
                  key={index}
                  className="bg-white rounded-xl p-6 shadow-lg hover:shadow-2xl hover:scale-105 transition-all border-2 border-gray-100"
                >
                  <div className="text-gray-900 mb-4 flex justify-center">
                    {feature.icon}
                  </div>
                  <h3 className="text-xl font-bold text-gray-900 mb-2 text-center">
                    {feature.title}
                  </h3>
                  <p className="text-gray-600 text-center">{feature.description}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Success Stories Gallery */}
          <div className="bg-gray-50 rounded-3xl p-8 shadow-xl">
            <h2 className="text-4xl font-bold text-center text-gray-900 mb-4">
              Thành Công Của Học Viên
            </h2>
            <p className="text-center text-gray-600 mb-12 text-lg">
              Hơn 5,000+ học viên đã đạt mục tiêu với Huyen Pham Language Center
            </p>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {[1, 2, 3].map((item) => (
                <div key={item} className="bg-white rounded-2xl overflow-hidden shadow-lg hover:shadow-2xl transition-all">
                  <div className="aspect-square bg-gray-200 flex items-center justify-center">
                    <div className="text-center p-6">
                      <img src={item === 1 ? Asset.student1 : item === 2 ? Asset.student2 : Asset.student3} alt={`Học viên ${item}`} className="object-cover mx-auto" />
                    </div>
                  </div>
                  <div className="p-6">
                    <div className="flex items-center mb-3">
                      {[...Array(5)].map((_, i) => (
                        <span key={i} className="text-gray-900 text-xl">★</span>
                      ))}
                    </div>
                    <p className="text-gray-700 italic mb-3">
                      "Tôi đã đạt 7.5 IELTS sau 5 tháng học. Giáo viên rất nhiệt tình và phương pháp giảng dạy hiệu quả!"
                    </p>
                    <p className="font-bold text-gray-900">Học viên #{item}</p>
                    <p className="text-sm text-gray-500">IELTS 7.5</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Certificates & Partners */}
          <div className="text-center bg-gradient-to-br from-gray-50 to-white rounded-3xl p-8 shadow-xl">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Đối Tác & Chứng Nhận
            </h2>
            <p className="text-gray-600 mb-12 text-lg">
              Được công nhận bởi các tổ chức uy tín quốc tế
            </p>
            <div className="bg-white rounded-3xl p-8 shadow-xl border-2 border-gray-100">
              <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
                {partners.map((partner, index) => (
                  <div key={index} className="aspect-square bg-gray-100 rounded-2xl flex items-center justify-center hover:scale-105 transition-all">
                    <div className="text-center p-4 flex flex-col h-full w-full">
                      <div className="w-full flex-1 flex items-center justify-center p-4">
                      <img src={partner.logo} alt={partner.name} className="w-full h-full object-contain" style={{ minWidth: '80px', minHeight: '80px' }} />
                      </div>
                      <p className="font-bold text-gray-900 mt-2">{partner.name}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Registration Form */}
          <div id="register" className="bg-gradient-to-br from-amber-50 via-yellow-50 to-white rounded-3xl p-8 md:p-12 shadow-xl">
            <h2 className="text-4xl font-bold text-center text-gray-900 mb-8">
              Đăng ký tư vấn khoá học
            </h2>
            
            <div className="max-w-6xl mx-auto bg-white rounded-3xl shadow-2xl overflow-hidden">
              <div className="grid md:grid-cols-2 gap-0">
                {/* Form Section */}
                <div className="p-8 md:p-12">
                  {showSuccess && (
                    <div className="bg-green-500 text-white p-4 rounded-lg mb-6 flex items-center">
                      <Check className="w-6 h-6 mr-2" />
                      Đăng ký thành công! Chúng tôi sẽ liên hệ bạn sớm nhất.
                    </div>
                  )}

                  <div className="space-y-5">
                    <div className="grid md:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-gray-700 font-semibold mb-2">
                          Họ tên <span className="text-red-500">*</span>
                        </label>
                        <input
                          type="text"
                          name="hoTen"
                          value={formData.hoTen}
                          onChange={handleInputChange}
                          placeholder="Nhập họ tên của bạn"
                          className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                        />
                      </div>

                      <div>
                        <label className="block text-gray-700 font-semibold mb-2">
                          Email <span className="text-red-500">*</span>
                        </label>
                        <input
                          type="email"
                          name="email"
                          value={formData.email}
                          onChange={handleInputChange}
                          placeholder="Nhập email của bạn"
                          className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                        />
                      </div>
                    </div>

                    <div className="grid md:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-gray-700 font-semibold mb-2">
                          Số điện thoại <span className="text-red-500">*</span>
                        </label>
                        <input
                          type="tel"
                          name="soDienThoai"
                          value={formData.soDienThoai}
                          onChange={handleInputChange}
                          placeholder="Nhập số điện thoại của bạn"
                          className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                        />
                      </div>

                      <div>
                        <label className="block text-gray-700 font-semibold mb-2">
                          Giới tính <span className="text-red-500">*</span>
                        </label>
                        <select
                          name="gioiTinh"
                          value={formData.gioiTinh}
                          onChange={handleInputChange}
                          className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                        >
                          <option value="">Chọn giới tính</option>
                          <option value={0}>Nam</option>
                          <option value={1}>Nữ</option>
                        </select>
                      </div>
                    </div>
                    <div>
                        <label className="block text-gray-700 font-semibold mb-2">
                          Khoá học quan tâm <span className="text-red-500">*</span>
                        </label>
                        <select
                          name="khoaHoc"
                          value={formData.khoaHoc}
                          onChange={handleInputChange}
                          className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                        >
                          <option value="">Chọn khoá học quan tâm</option>
                          {courses.map((course, index) => (
                            <option key={index} value={course.id}>
                              {course.title}
                            </option>
                          ))}
                        </select>
                      </div>
                    <div>
                      <label className="block text-gray-700 font-semibold mb-2">
                        Nội dung cần tư vấn
                      </label>
                      <textarea
                        name="noiDung"
                        value={formData.noiDung}
                        onChange={handleInputChange}
                        rows={4}
                        placeholder="Nhập nội dung"
                        className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors resize-none"
                      />
                    </div>

                    <button
                      onClick={handleSubmit}
                      className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all shadow-lg uppercase"
                    >
                      Đăng ký
                    </button>
                  </div>
                </div>

                {/* Illustration Section */}
                <div className="hidden md:flex items-center justify-center bg-gradient-to-br from-purple-100 via-blue-50 to-purple-50 p-12">
                  <div className="relative w-full max-w-md">
                    <div className="text-center">
                      <div className="text-8xl mb-4">👨‍🏫</div>
                      <h3 className="text-2xl font-bold text-gray-800 mb-2">Tư vấn miễn phí</h3>
                      <p className="text-gray-600">Chúng tôi sẽ liên hệ bạn trong 24h</p>
                      
                      {/* Decorative elements */}
                      <div className="absolute top-0 right-0 w-20 h-20 bg-purple-200 rounded-full opacity-50 animate-pulse"></div>
                      <div className="absolute bottom-0 left-0 w-16 h-16 bg-blue-200 rounded-full opacity-50 animate-pulse delay-75"></div>
                      <div className="absolute top-1/2 left-0 w-12 h-12 bg-purple-300 rounded-full opacity-30 animate-bounce"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </>
  );
}