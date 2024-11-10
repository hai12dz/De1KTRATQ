using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace de1
{
    public partial class Form1 : Form
    {
        string connectString = @"Data Source=hai\SQLEXPRESS;Initial Catalog=de1;Integrated Security=True;Encrypt=False";

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();
            con = new SqlConnection(connectString);
            loadData();

            // Thiết lập trạng thái nút ban đầu
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnThem.Enabled = true;
            btnBoQua.Enabled = true;
        }

        private void loadData()
        {
            try
            {
                dt.Clear();
                con.Open(); // Mở kết nối thủ công
                cmd = new SqlCommand("SELECT * FROM VatLieu", con);
                adt = new SqlDataAdapter(cmd);
                adt.Fill(dt);
                con.Close(); // Đóng kết nối thủ công
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }


        private void btnAnh_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog1.Title = "Chọn ảnh đại diện";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void giaNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void giaBan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void soLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu.");
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaVL"].ToString() == textBox1.Text)
                {
                    MessageBox.Show("Mã vật liệu đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }
            }

            try
            {
                con.Open(); // Mở kết nối thủ công
                cmd = new SqlCommand("INSERT INTO VatLieu (MaVL, TenVatLieu, DonViTinh, GiaNhap, GiaBan, SoLuong, Anh, GhiChu) VALUES (@MaVL, @TenVatLieu, @DonViTinh, @GiaNhap, @GiaBan, @SoLuong, @Anh, @GhiChu)", con);
                cmd.Parameters.AddWithValue("@MaVL", textBox1.Text);
                cmd.Parameters.AddWithValue("@TenVatLieu", textBox2.Text);
                cmd.Parameters.AddWithValue("@DonViTinh", textBox3.Text);
                cmd.Parameters.AddWithValue("@GiaNhap", giaNhap.Text);
                cmd.Parameters.AddWithValue("@GiaBan", giaBan.Text);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong.Text);
                cmd.Parameters.AddWithValue("@Anh", openFileDialog1.FileName);
                cmd.Parameters.AddWithValue("@GhiChu", textBoxGhiChu.Text);

                cmd.ExecuteNonQuery();
                con.Close(); // Đóng kết nối thủ công

                MessageBox.Show("Thêm vật liệu thành công!");
                loadData();
                clearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vật liệu: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["MaVL"].Value.ToString();
                textBox2.Text = row.Cells["TenVatLieu"].Value.ToString();
                textBox3.Text = row.Cells["DonViTinh"].Value.ToString();
                giaNhap.Text = row.Cells["GiaNhap"].Value.ToString();
                giaBan.Text = row.Cells["GiaBan"].Value.ToString();
                soLuong.Text = row.Cells["SoLuong"].Value.ToString();
                textBoxGhiChu.Text = row.Cells["GhiChu"].Value?.ToString(); // Hiển thị ghi chú

                if (row.Cells["Anh"].Value != DBNull.Value)
                {
                    pictureBox1.Image = Image.FromFile(row.Cells["Anh"].Value.ToString());
                }

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnThem.Enabled = false;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Hiển thị thông báo xác nhận khi người dùng chọn xóa
            var result = MessageBox.Show("Bạn có muốn xóa vật liệu này không?", "Xác nhận", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    con.Open(); // Mở kết nối thủ công
                    cmd = new SqlCommand("DELETE FROM VatLieu WHERE MaVL = @MaVL", con);
                    cmd.Parameters.AddWithValue("@MaVL", textBox1.Text);
                    cmd.ExecuteNonQuery();
                    con.Close(); // Đóng kết nối thủ công

                    MessageBox.Show("Xóa vật liệu thành công!");

                    // Cập nhật lại lưới dữ liệu
                    loadData();

                    // Xóa rỗng các TextBox và ảnh đại diện
                    clearTextBoxes();

                    // Cập nhật lại trạng thái nút
                    btnSua.Enabled = false;
                    btnXoa.Enabled = false;
                    btnThem.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa vật liệu: " + ex.Message);
                }
            }
            else if (result == DialogResult.No)
            {
                // Nếu chọn "No", không xóa và thực hiện các bước sau

                // Xóa rỗng các TextBox và ảnh đại diện
                clearTextBoxes();

                // Cập nhật lại trạng thái nút
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnThem.Enabled = true;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy ảnh cũ từ cơ sở dữ liệu trước khi cập nhật
                string currentImagePath = GetCurrentImagePath(textBox1.Text);

                // Kiểm tra xem người dùng có chọn ảnh mới không
                string imagePath = openFileDialog1.FileName;

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                if (string.IsNullOrEmpty(imagePath) || imagePath == "openFileDialog1")
                {
                    imagePath = currentImagePath;
                }

                con.Open(); // Mở kết nối thủ công

                // Cập nhật thông tin vật liệu vào cơ sở dữ liệu
                cmd = new SqlCommand("UPDATE VatLieu SET TenVatLieu = @TenVatLieu, DonViTinh = @DonViTinh, GiaNhap = @GiaNhap, GiaBan = @GiaBan, SoLuong = @SoLuong, Anh = @Anh, GhiChu = @GhiChu WHERE MaVL = @MaVL", con);


                cmd.Parameters.AddWithValue("@MaVL", textBox1.Text);
                cmd.Parameters.AddWithValue("@TenVatLieu", textBox2.Text);
                cmd.Parameters.AddWithValue("@DonViTinh", textBox3.Text);
                cmd.Parameters.AddWithValue("@GiaNhap", giaNhap.Text);
                cmd.Parameters.AddWithValue("@GiaBan", giaBan.Text);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong.Text);
                cmd.Parameters.AddWithValue("@GhiChu", textBoxGhiChu.Text);

                // Cập nhật ảnh: nếu có ảnh mới, dùng ảnh mới; nếu không có, giữ ảnh cũ
                if (string.IsNullOrEmpty(imagePath))
                {
                    cmd.Parameters.AddWithValue("@Anh", DBNull.Value);  // Nếu không có ảnh cũ hoặc mới, đặt giá trị ảnh là NULL
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Anh", imagePath);  // Cập nhật ảnh mới hoặc giữ ảnh cũ
                }

                cmd.ExecuteNonQuery();
                con.Close(); // Đóng kết nối thủ công

                MessageBox.Show("Cập nhật vật liệu thành công!");
                loadData();
                clearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật vật liệu: " + ex.Message);
            }
        }

        private string GetCurrentImagePath(string maVL)
        {
            string currentImagePath = null;

            try
            {
                using (SqlConnection con = new SqlConnection(connectString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Anh FROM VatLieu WHERE MaVL = @MaVL", con))
                    {
                        cmd.Parameters.AddWithValue("@MaVL", maVL);
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            currentImagePath = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy ảnh cũ: " + ex.Message);
            }

            return currentImagePath;
        }


        private void clearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBoxGhiChu.Clear();
            giaNhap.Clear();
            giaBan.Clear();
            soLuong.Clear();
            pictureBox1.Image = null;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnThem.Enabled = true;
        }
    }
}
