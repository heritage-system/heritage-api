
DELETE FROM HeritageLocations;
DELETE FROM Heritages;
DELETE FROM Categories;
DELETE FROM Locations;
-- =====================
-- 1. Categories
-- =====================
INSERT INTO Categories (name, description, created_by, create_at, update_at)
VALUES (N'Lễ hội truyền thống', N'Di sản phi vật thể - lễ hội truyền thống', 'system', GETDATE(), GETDATE());
GO

-- =====================
-- 2. Heritages
-- =====================
INSERT INTO Heritages 
(name, description, name_unsigned, description_unsigned, category_id, map_url, is_featured, created_by, create_at, update_at)
VALUES
(N'Lễ hội Miếu Bà Rá', 
 N'Núi Bà Rá gắn với truyền thuyết Bà Rá và Bà Đen (Tây Ninh); lễ hội thu hút hàng nghìn người dân và du khách hành hương mỗi năm', 
 N'le hoi mieu ba ra', 
 N'nui ba ra gan voi truyen thuyet ba ra va ba den tay ninh; le hoi thu hut hang nghin nguoi dan va du khach hanh huong moi nam', 
 1, 
 N'https://baobinhphuoc.com.vn/news/19/116116/le-hoi-mieu-ba-ra-giap-thin-dien-ra-tu-ngay-9-den-11-4-2024', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Cầu ngư ở Vạn Thủy Tú', 
 N'Lăng Vạn Thủy Tú được xây dựng từ năm 1762, hiện lưu giữ bộ xương cá voi lớn nhất Đông Nam Á. Lễ hội Cầu ngư phản ánh sinh hoạt văn hóa - tâm linh đặc trưng của cư dân ven biển Nam Trung Bộ.', 
 N'le hoi cau ngu o van thuy tu', 
 N'lang van thuy tu duoc xay dung tu nam 1762, hien luu giu bo xuong ca voi lon nhat dong nam a. le hoi cau ngu phan anh sinh hoat van hoa - tam linh dac trung cua cu dan ven bien nam trung bo.', 
 1, 
 N'https://svhttdl.binhthuan.gov.vn/di-tich-danh-thang-le-hoi-van-hoa/le-hoi-cau-ngu-van-thuy-tu-thanh-pho-phan-thiet-tinh-binh-thuan-639496', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Chùa Láng', 
 N'Lễ hội Chùa Láng từng được tổ chức trong 10 ngày với sự tham gia của 9 làng, là hội lớn nhất phía Tây kinh thành Thăng Long xưa.', 
 N'le hoi chua lang', 
 N'le hoi chua lang tung duoc to chuc trong 10 ngay voi su tham gia cua 9 lang, la hoi lon nhat phia tay kinh thanh thang long xua.', 
 1, 
 N'https://vinpearl.com/vi/kham-pha-le-hoi-chua-lang', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Chùa Đọi Sơn', 
 N'Lễ hội được duy trì và phát triển, trở thành một trong những lễ hội tiêu biểu của tỉnh Hà Nam.', 
 N'le hoi chua doi son', 
 N'le hoi duoc duy tri va phat trien, tro thanh mot trong nhung le hoi tieu bieu cua tinh ha nam.', 
 1, 
 N'https://baohanam.com.vn/van-hoa/le-hoi/le-hoi-truyen-thong-chua-long-doi-son-nam-2024-122101.html', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đền thờ Trạng Trình Nguyễn Bỉnh Khiêm', 
 N'Lễ hội được tổ chức thường niên và đã được công nhận là Di sản văn hóa phi vật thể quốc gia vào năm 2019.', 
 N'le hoi den tho trang trinh nguyen binh khiem', 
 N'le hoi duoc to chuc thuong nien va da duoc cong nhan la di san van hoa phi vat the quoc gia vao nam 2019.', 
 1, 
 N'https://vov.vn/multimedia/anh/ron-rang-le-hoi-den-trang-trinh-nguyen-binh-khiem-post1070110.vov', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đền Chu Hưng', 
 N'Đền Chu Hưng từng là nơi diễn ra nhiều sự kiện lịch sử quan trọng, như nơi tuyên bố thành lập các đoàn thể cứu quốc sau Cách mạng Tháng Tám năm 1941.', 
 N'le hoi den chu hung', 
 N'den chu hung tung la noi dien ra nhieu su kien lich su quan trong, nhu noi tuyen bo thanh lap cac doan the cuu quoc sau cach mang thang tam nam 1941.', 
 1, 
 N'https://baophutho.vn/gia-tri-lich-su-cua-den-chu-hung-169132.htm', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đền Mưng', 
 N'Lễ hội truyền thống tại Thanh Hóa, đang được bảo tồn.', 
 N'le hoi den mung', 
 N'le hoi truyen thong tai thanh hoa, dang duoc bao ton.', 
 1, 
 N'https://baothanhhoa.vn/le-hoi-den-mung-nbsp-net-dep-truyen-thong-dam-da-ban-sac-xu-thanh-140397.htm', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội mường Ca Da', 
 N'Lễ hội của cộng đồng người Thái tại Thanh Hóa, thể hiện bản sắc truyền thống.', 
 N'le hoi muong ca da', 
 N'le hoi cua cong dong nguoi thai tai thanh hoa, the hien ban sac truyen thong.', 
 1, 
 N'https://truyenhinhthanhhoa.vn/le-hoi-muong-ca-da-noi-toa-sang-cac-gia-tri-van-hoa-truyen-thong-110230327111223007.htm', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đình Quan Lạn', 
 N'Lễ hội truyền thống tại xã Quan Lạn, huyện Vân Đồn, Quảng Ninh.', 
 N'le hoi dinh quan lan', 
 N'le hoi truyen thong tai xa quan lan, huyen van don, quang ninh.', 
 1, 
 N'https://www.qdnd.vn/van-hoa/doi-song/doc-dao-le-hoi-truyen-thong-van-don-le-hoi-dinh-quan-lan-700140', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đình Trà Cổ', 
 N'Lễ hội tại Đình Trà Cổ, di tích lịch sử văn hóa cấp quốc gia đặc biệt.', 
 N'le hoi dinh tra co', 
 N'le hoi tai dinh tra co, di tich lich su van hoa cap quoc gia dac biet.', 
 1, 
 N'https://mongcai.gov.vn/vi-vn/tin/le-hoi-dinh-tra-co--di-san-van-hoa-phi-vat-the-cap-quoc-gia-p16129-c76190-n249699', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Ada Koohn (Mừng lúa mới) của người Pa Cô', 
 N'Tết mừng lúa mới – dịp sum họp của người Pa Cô tại huyện A Lưới, Huế.', 
 N'le hoi ada koohn mung lua moi cua nguoi pa co', 
 N'tet mung lua moi – dip sum hop cua nguoi pa co tai huyen a luoi, hue.', 
 1, 
 N'https://www.vietnamplus.vn/doc-dao-le-cung-mung-lua-moi-cua-nguoi-dan-toc-pa-co-o-thua-thien-hue-post999111.vnp', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Chùa Đại Bi', 
 N'Chùa Đại Bi là ngôi chùa cổ, có lịch sử lâu đời, gắn liền với truyền thống Phật giáo.', 
 N'le hoi chua dai bi', 
 N'chua dai bi la ngoi chua co, co lich su lau doi, gan lien voi truyen thong phat giao.', 
 1, 
 N'https://hoabinhtourist.com/cam-nang-du-lich/le-hoi-chua-bai-dinh', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Đền, Chùa Linh Quang', 
 N'Đền, Chùa Linh Quang là di tích lịch sử có giá trị văn hóa đặc biệt tại Nam Định.', 
 N'le hoi den, chua linh quang', 
 N'den, chua linh quang la di tich lich su co gia tri van hoa dac biet tai nam dinh.', 
 1, 
 N'https://phuongdinh.namdinh.gov.vn/tin-hoat-dong/le-don-bang-chung-nhan-le-hoi-den-chua-linh-quang-xa-phuong-dinh-duoc-dua-vao-danh-muc-di-san-va-216744', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội làng Chử Xá', 
 N'Lễ hội gắn với truyền thống văn hóa của cư dân làng Chử Xá, Hà Nội.', 
 N'le hoi lang chu xa', 
 N'le hoi gan voi truyen thong van hoa cua cu dan lang chu xa, ha noi.', 
 1, 
 N'https://nguoihanoi.vn/le-hoi-lang-chu-xa-la-di-san-van-hoa-phi-vat-the-quoc-gia-67479.html', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Gầu tào của người Mông', 
 N'Lễ hội mang đậm nét văn hóa dân tộc Mông tại Lai Châu.', 
 N'le hoi gau tao cua nguoi mong', 
 N'le hoi mang dam net van hoa dan toc mong tai lai chau.', 
 1, 
 N'https://nhandan.vn/dac-sac-van-hoa-mong-qua-le-hoi-gau-tao-post796616.html', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Lăng Ông Trà Ôn', 
 N'Lễ hội tưởng nhớ công lao Đức Thánh Trần, Vĩnh Long.', 
 N'le hoi lang ong tra on', 
 N'le hoi tuong nho cong lao duc thanh tran, vinh long.', 
 1, 
 N'https://vinhlongtourist.vn/en/detailnews/?id=news_12010&t=dac-sac-le-hoi-lang-ong-tien-quan-thong-che-dieu-bat', 
 0, 'system', GETDATE(), GETDATE()),

(N'Lễ hội Tranh đầu pháo', 
 N'Lễ hội dân gian độc đáo tại Quảng Uyên, Cao Bằng.', 
 N'le hoi tranh dau phao', 
 N'le hoi dan gian doc dao tai quang uyen, cao bang.', 
 1, 
 N'https://baove.congly.vn/cao-bang-doc-dao-net-dep-van-hoa-dan-toc-trong-le-hoi-tranh-dau-phao-tai-quang-uyen-420993.html', 
 0, 'system', GETDATE(), GETDATE());
GO


-- =====================
-- 3. Locations + HeritageLocations
-- =====================
 --  Lễ hội 2: Thị xã Phước Long, Bình Phước
INSERT INTO Locations
    (province, district, ward, addressDetail, latitude, longitude, province_unsigned, district_unsigned, ward_unsigned, address_detail_unsigned, created_by, updated_by, create_at, update_at)
VALUES
    (N'Bình Phước', N'Thị xã Phước Long', NULL, NULL, 11.8012, 106.7745,
     N'binh phuoc', N'thi xa phuoc long', NULL, NULL,
     'system', 'system', GETDATE(), GETDATE());

DECLARE @loc1 INT = SCOPE_IDENTITY();

INSERT INTO HeritageLocations (heritage_id, location_id, created_by, create_at, update_at) 
VALUES (2, @loc1, 'system', GETDATE(), GETDATE());

 --  Lễ hội 3: Phường Đức Thắng, Phan Thiết, Bình Thuận
INSERT INTO Locations
    (province, district, ward, addressDetail, latitude, longitude, province_unsigned, district_unsigned, ward_unsigned, address_detail_unsigned, created_by, updated_by, create_at, update_at)
VALUES
    (N'Bình Thuận', N'Phan Thiết', N'Phường Đức Thắng', NULL, 10.9453, 108.0836,
     N'binh thuan', N'phan thiet', N'phuong duc thang', NULL,
     'system', 'system', GETDATE(), GETDATE());

DECLARE @loc2 INT = SCOPE_IDENTITY();

INSERT INTO HeritageLocations (heritage_id, location_id, created_by, create_at, update_at) 
VALUES (2, @loc2, 'system', GETDATE(), GETDATE());

 --  Lễ hội 4: Lễ hội Lim, Bắc Ninh
INSERT INTO Locations
    (province, district, ward, addressDetail, latitude, longitude, province_unsigned, district_unsigned, ward_unsigned, address_detail_unsigned, created_by, updated_by, create_at, update_at)
VALUES
    (N'Bắc Ninh', N'Từ Sơn', N'Xã Lim', NULL, 21.1234, 106.1234,
     N'bac ninh', N'tu son', N'xa lim', NULL,
     'system', 'system', GETDATE(), GETDATE());

DECLARE @loc3 INT = SCOPE_IDENTITY();

INSERT INTO HeritageLocations (heritage_id, location_id, created_by, create_at, update_at)
VALUES (4, @loc3, 'system', GETDATE(), GETDATE());

 --  Lễ hội 5: Lễ hội Cốm Vòng, Hà Nội
INSERT INTO Locations
    (province, district, ward, addressDetail, latitude, longitude, province_unsigned, district_unsigned, ward_unsigned, address_detail_unsigned, created_by, updated_by, create_at, update_at)
VALUES
    (N'Hà Nội', N'Thạch Thất', N'Xã Vân Cốc', 'Làng Cốm Vòng', 21.0356, 105.6243,
     N'ha noi', N'thach that', N'xa van coc', N'lang com vong',
     'system', 'system', GETDATE(), GETDATE());

DECLARE @loc4 INT = SCOPE_IDENTITY();

INSERT INTO HeritageLocations (heritage_id, location_id, created_by, create_at, update_at)
VALUES (6, @loc4, 'system', GETDATE(), GETDATE());

  -- Lễ hội 6: Lễ hội Chọi Trâu, Hải Phòng
INSERT INTO Locations
    (province, district, ward, addressDetail, latitude, longitude, province_unsigned, district_unsigned, ward_unsigned, address_detail_unsigned, created_by, updated_by, create_at, update_at)
VALUES
    (N'Hải Phòng', N'Hồng Bàng', NULL, 'Sân đấu Chọi Trâu', 20.8501, 106.6804,
     N'hai phong', N'hong bang', NULL, N'san dau choi trau',
     'system', 'system', GETDATE(), GETDATE());

DECLARE @loc5 INT = SCOPE_IDENTITY();

INSERT INTO HeritageLocations (heritage_id, location_id, created_by, create_at, update_at)
VALUES (6, @loc5, 'system', GETDATE(), GETDATE());


INSERT INTO Tags (name, created_by, create_at, update_at)
VALUES
(N'Lễ hội truyền thống', 'system', GETDATE(), GETDATE()),
(N'Lễ hội mùa xuân', 'system', GETDATE(), GETDATE()),
(N'Lễ hội mùa thu', 'system', GETDATE(), GETDATE()),
(N'Lễ hội làng', 'system', GETDATE(), GETDATE()),
(N'Lễ hội dân gian', 'system', GETDATE(), GETDATE()),
(N'Lễ hội tôn giáo', 'system', GETDATE(), GETDATE()),
(N'Lễ hội văn hóa', 'system', GETDATE(), GETDATE());

INSERT INTO HeritageTags (heritage_id,tag_id, created_by, create_at, update_at)
VALUES
(1,1, 'system', GETDATE(), GETDATE()),
(1,2, 'system', GETDATE(), GETDATE()),
(2,2, 'system', GETDATE(), GETDATE()),
(3,4, 'system', GETDATE(), GETDATE()),
(2,4, 'system', GETDATE(), GETDATE()),
(5,3, 'system', GETDATE(), GETDATE()),
(7,1, 'system', GETDATE(), GETDATE());

INSERT INTO HeritageMedias
    (heritage_id, media_type, url, description, created_by, updated_by, create_at, update_at)
VALUES
    (1, N'IMAGE', N'https://example.com/IMAGEs/giong.jpg', N'Hình ảnh Lễ hội Gióng', 'system', 'system', GETDATE(), GETDATE()),
    (1, N'VIDEO', N'https://example.com/VIDEOs/giong.mp4', N'VIDEO Lễ hội Gióng', 'system', 'system', GETDATE(), GETDATE()),
    (2, N'IMAGE', N'https://example.com/IMAGEs/huong.jpg', N'Hình ảnh Lễ hội Chùa Hương', 'system', 'system', GETDATE(), GETDATE()),
    (3, N'IMAGE', N'https://example.com/IMAGEs/hung.jpg', N'Hình ảnh Lễ hội Đền Hùng', 'system', 'system', GETDATE(), GETDATE()),
    (3, N'VIDEO', N'https://example.com/VIDEOs/hung.mp4', N'VIDEO Lễ hội Đền Hùng', 'system', 'system', GETDATE(), GETDATE()),
    (4, N'IMAGE', N'https://example.com/IMAGEs/lim.jpg', N'Hình ảnh Lễ hội Lim', 'system', 'system', GETDATE(), GETDATE()),
    (5, N'IMAGE', N'https://example.com/IMAGEs/com.jpg', N'Hình ảnh Lễ hội Cốm Vòng', 'system', 'system', GETDATE(), GETDATE()),
    (6, N'IMAGE', N'https://example.com/IMAGEs/trau.jpg', N'Hình ảnh Lễ hội Chọi Trâu', 'system', 'system', GETDATE(), GETDATE());



GO

-- =====================
-- 5. HeritageOccurrences
-- =====================
INSERT INTO HeritageOccurrences 
(heritage_id, occurrence_type, calendar_type,
 start_day, start_month, end_day, end_month,
 frequency, description, created_by, create_at, update_at)
VALUES

-- Lễ hội Miếu Bà Rá (10/3 âm lịch hàng năm)
(1, N'EXACTDATE', N'LUNAR',
10, 3, NULL, NULL,
N'ANNUAL',
N'Được duy trì đều đặn hàng năm, có sự tham gia của chính quyền địa phương. Quyết định: 4600/QĐ-BVHTTDL ngày 20/12/2019. Hoạt động: Cúng tế, dâng hương, múa lân, trò chơi dân gian. Chủ thể: Các bô lão, Ban Quản lý Miếu. Cộng đồng: dân cư Thị xã Phước Long.',
'system', GETDATE(), GETDATE()),

-- Lễ hội Cầu ngư Vạn Thủy Tú (15–16/6 âm lịch hàng năm)
(2, N'RANGE', N'LUNAR',
15, 6, 16, 6,
N'ANNUAL',
N'Điểm nhấn văn hóa - du lịch của Phan Thiết. Quyết định: 4614/QĐ-BVHTTDL ngày 20/12/2019. Hoạt động: Lễ nghi dân gian, rước thần, hát bội, trò chơi dân gian. Chủ thể: Các bô lão, Ban nghi lễ lăng Vạn Thủy Tú. Cộng đồng: cư dân Đức Thắng, Phan Thiết.',
'system', GETDATE(), GETDATE());
GO
