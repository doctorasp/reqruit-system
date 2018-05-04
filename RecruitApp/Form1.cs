using PersonalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RecruitApp
{
    public partial class Form1 : Form
    {
        MySQLContext _context;
        public Form1()
        {
            _context = new MySQLContext();
            InitializeComponent();
        }

        private void drawRows()
        {
            for (int i = 0; i < this.dataGridView1.RowCount - 1; i++)
            {

                if (this.dataGridView1.Rows[i].Cells[7].Value != null)
                {
                    if (Convert.ToDecimal(this.dataGridView1.Rows[i].Cells[7].Value) > 1)
                    {
                        this.dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.Orange;
                    }
                    else
                    {
                        this.dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.LightSeaGreen;
                    }

                }

            }
        }

        private void mainTable()
        {
            this.dataGridView1.DataSource = _context.GetQuery("SELECT person.id AS 'ID', fio AS 'ПІП', phone AS 'Телефон', city AS 'Місто', vacancy.name AS 'Вакансія', target_price AS 'Очікувана ЗП, $', vacancy.price AS 'Оклад на позиції', priceK AS 'ЗПК', score AS 'K вмінь', s.sum_c AS 'Стаж, р.' FROM person LEFT JOIN (SELECT id_person, sum(Floor(DATEDIFF(date_end, date_start))/365) sum_c FROM experience GROUP BY experience.id_person) s ON person.id = s.id_person LEFT JOIN vacancy ON person.id_vacancy = vacancy.id INNER JOIN rating ON person.id = rating.id_person ORDER BY rating.score AND rating.priceK DESC ");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainTable();
            drawRows();
            this.dataGridView3.DataSource = _context.GetQuery("SELECT id AS 'ID', name AS 'Назва вакансії', price AS 'Оклад позиції', isClose AS 'Статус' FROM vacancy");
            if (this.dataGridView1.RowCount > 0)
            {
                this.dataGridView1.Columns[0].Visible = false;
            }

            DataTable dt = _context.GetQuery("SELECT * FROM vacancy WHERE isClose = 0");

            this.comboBox1.DataSource = dt;
            this.comboBox1.DisplayMember = "name";
            this.comboBox1.ValueMember = "id";

            this.label16.Text = $"Всього персоналу:{this.dataGridView1.RowCount-1}";
        }


        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Font TabFont;
            Brush BackBrush = new SolidBrush(Color.White); //Set background color

            Brush ForeBrush = new SolidBrush(Color.Black);//Set foreground color

            if (e.Index == this.tabControl1.SelectedIndex)
            {
                TabFont = new Font(e.Font, FontStyle.Regular);
            }
            else
            {
                TabFont = e.Font;
            }
            string TabName = this.tabControl1.TabPages[e.Index].Text;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.FillRectangle(BackBrush, e.Bounds);
            Rectangle r = e.Bounds;
            r = new Rectangle(r.X, r.Y + 3, r.Width, r.Height - 3);
            e.Graphics.DrawString(TabName, TabFont, ForeBrush, r, sf);

            sf.Dispose();
            if (e.Index == this.tabControl1.SelectedIndex)
            {
                TabFont.Dispose();
                BackBrush.Dispose();
            }
            else
            {
                BackBrush.Dispose();
                ForeBrush.Dispose();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Додати вакансію?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string name = textBox5.Text;
                string price = textBox6.Text;
                int close = 0;
                _context.GetQuery("INSERT INTO vacancy(name, price, isClose) VALUES ('"+name+"', '"+price+"', '"+close+"')");
                this.dataGridView3.DataSource = _context.GetQuery("SELECT * FROM vacancy");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Ви хочете видалити запис?", "Confirmation", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
                _context.GetQuery("DELETE FROM vacancy WHERE id = '"+id+"'");
                this.dataGridView3.DataSource = _context.GetQuery("SELECT * FROM vacancy");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Закрити обрану вакансію?", "Confirmation", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
                _context.GetQuery("UPDATE vacancy SET isClose = 1 WHERE id = '"+id+"'");
                this.dataGridView3.DataSource = _context.GetQuery("SELECT * FROM vacancy");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Відкрити обрану вакансію?", "Confirmation", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
                _context.GetQuery("UPDATE vacancy SET isClose = 0 WHERE id = '" + id + "'");
                this.dataGridView3.DataSource = _context.GetQuery("SELECT * FROM vacancy");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string pip = this.textBox1.Text;
            string phone = this.textBox2.Text;
            string city = this.textBox3.Text;
            int id_v = (int)this.comboBox1.SelectedValue;

            try
            {
                _context.GetQuery("INSERT INTO person (fio, phone, city, id_vacancy) VALUES ('" + pip + "', '" + phone + "', '" + city + "','"+id_v+"')");
                DataTable dt = _context.GetQuery("SELECT id FROM person ORDER BY id DESC LIMIT 1");
                for (int i = 0; i < this.dataGridView2.RowCount - 1; i++)
                {
                    _context.GetQuery("UPDATE experience SET id_person = '" + dt.Rows[0][0] + "' WHERE id_person = 0 AND id = '" + this.dataGridView2.Rows[i].Cells[0].Value + "'");
                }
                MessageBox.Show("Данні успішно збережено");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.InnerException+"");
            }
           
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string pip = this.textBox1.Text;
            string phone = this.textBox2.Text;
            string city = this.textBox3.Text;
            int id_v = (int)this.comboBox1.SelectedValue;
            decimal target_price=0;

            if (!String.IsNullOrEmpty(this.textBox8.Text))
            {
                target_price = Convert.ToDecimal(this.textBox8.Text);
            }
            else
            {
                MessageBox.Show("Введіть очікуваний оклад");
                return;
            }

            DialogResult result = MessageBox.Show("Створити нового найманця?", "Confirmation", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {

                try
                {
                    _context.GetQuery("INSERT INTO person (fio, phone, city, id_vacancy, target_price) VALUES ('" + pip + "', '" + phone + "', '" + city + "','" + id_v + "', '" + target_price + "')");
                    DataTable dt = _context.GetQuery("SELECT id FROM person ORDER BY id DESC LIMIT 1");
                    int person_id = Convert.ToInt32(dt.Rows[0][0]);
                    for (int i = 0; i < this.dataGridView2.RowCount - 1; i++)
                    {
                        _context.GetQuery("UPDATE experience SET id_person = '" + person_id + "' WHERE id_person = 0 AND id = '" + this.dataGridView2.Rows[i].Cells[0].Value + "'");
                    }
                    DataTable v_price = _context.GetQuery("SELECT price FROM vacancy WHERE id = '" + id_v + "'");

                    decimal K = insertQualities(person_id);
                    decimal priceK = (decimal)target_price / (decimal)v_price.Rows[0][0];
                    _context.GetQuery("INSERT INTO rating (score, id_person, priceK) VALUES ('" + K + "', '" + person_id + "', '" + priceK + "')");
                    mainTable();
                    drawRows();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException + "");
                }
            }
        }

        public decimal insertQualities(int id_person)
        {
            int countSkills = 0;
            decimal qualityK = 0;
            int c = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (checkBox1.Checked && comboBox2.SelectedItem != null)
            {
                countSkills += Convert.ToInt32(comboBox2.SelectedItem.ToString());
                c++;
                dic.Add(checkBox1.Text, comboBox2.SelectedItem.ToString());
            }
            if (checkBox2.Checked && comboBox3.SelectedItem != null)
            {
                countSkills += Convert.ToInt32(comboBox3.SelectedItem.ToString());
                c++;
                dic.Add(checkBox2.Text, comboBox3.SelectedItem.ToString());
            }
            if (checkBox3.Checked && comboBox4.SelectedItem != null)
            {
                countSkills += Convert.ToInt32(comboBox4.SelectedItem.ToString());
                c++;
                dic.Add(checkBox3.Text, comboBox4.SelectedItem.ToString());
            }

            if (checkBox4.Checked && comboBox5.SelectedItem != null)
            {
                c++;
                countSkills += Convert.ToInt32(comboBox5.SelectedItem.ToString());
                dic.Add(checkBox4.Text, comboBox5.SelectedItem.ToString());
            }

            if (checkBox5.Checked && comboBox6.SelectedItem != null)
            {
                c++;
                dic.Add(checkBox5.Text, comboBox6.SelectedItem.ToString());
            }

            if (checkBox6.Checked)
            {
                c++;
                dic.Add(checkBox6.Text, "");
            }

            if (checkBox7.Checked)
            {
                c++;
                dic.Add(checkBox7.Text, "");
            }

            qualityK = (decimal)countSkills / (decimal)c;

            foreach(var i in dic)
            {
                _context.GetQuery("INSERT INTO quality (skill_name, id_person, point) VALUES('"+i.Key+"','"+id_person+"','"+i.Value+"')");
            }
            return qualityK ;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string work_place = textBox4.Text;
            string dateStart = this.dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string dateEnd = this.dateTimePicker2.Value.ToString("yyyy-MM-dd");
            string posada = textBox7.Text;

            _context.GetQuery("INSERT INTO experience (work_place, position, date_start, date_end) VALUES ('" + work_place + "', '" + posada + "','" + dateStart + "', '" + dateEnd + "')");
            this.dataGridView2.DataSource = _context.GetQuery("SELECT id AS 'ID', work_place AS 'Місце роботи', position AS 'Позиція', date_start AS 'з', date_end AS 'до' FROM experience WHERE id_person = 0");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int id = (int)dataGridView2.CurrentRow.Cells[0].Value;
            _context.GetQuery("DELETE FROM experience WHERE id = '" + id + "'");
            this.dataGridView2.DataSource = _context.GetQuery("SELECT * FROM experience WHERE id_person = 0");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = this.dataGridView1.DataSource;
            bs.Filter = "ПІП like '%" + this.richTextBox1.Text + "%'";
            this.dataGridView1.DataSource = bs;
            drawRows();
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "";
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "Пошук";
            mainTable();
            drawRows();

        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id_row = (int)this.dataGridView1.CurrentRow.Cells[0].Value;

            //DataTable dt = _context.GetQuery("SELECT fio AS 'ПІП', s.sum_c FROM person LEFT JOIN (SELECT , id_person, sum( DATEDIFF(date_end, date_start)) sum_c FROM experience GROUP BY id_person  ) s ON person.id = s.id_person ");
            //this.dataGridView1.DataSource = dt;
        }

        private void checkBox8_Click(object sender, EventArgs e)
        {
            if (this.checkBox8.Checked)
            {

                this.dataGridView1.DataSource = _context.GetQuery("SELECT person.id AS 'ID', fio AS 'ПІП', phone AS 'Телефон', city AS 'Місто', vacancy.name AS 'Вакансія', target_price AS 'Очікувана ЗП, $', vacancy.price AS 'Оклад на позиції', priceK AS 'ЗПК', score AS 'K вмінь', s.sum_c AS 'Стаж, р.' FROM person LEFT JOIN (SELECT id_person, sum(Floor(DATEDIFF(date_end, date_start))/365) sum_c FROM experience GROUP BY experience.id_person) s ON person.id = s.id_person LEFT JOIN vacancy ON person.id_vacancy = vacancy.id INNER JOIN rating ON person.id = rating.id_person ORDER BY person.target_price DESC ");
                drawRows();
            }
            else
            {
                mainTable();
                drawRows();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count>0)
            {
                int person_id = (int)this.dataGridView1.CurrentRow.Cells[0].Value;
                DataTable dt = _context.GetQuery("SELECT skill_name, point FROM quality WHERE id_person = '" + person_id + "'");
                string s = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                        if (dt.Rows[i][j] != null)
                            s += dt.Rows[i][j] + "\n";
                }
                MessageBox.Show("Вміння та оцінки:\n"+s);
            }
            else
            {
                MessageBox.Show("Виберіть потрібний рядок!");
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.comboBox2.Enabled = true;
            }
            else
            {
                this.comboBox2.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.comboBox3.Enabled = true;
            }
            else
            {
                this.comboBox3.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.comboBox4.Enabled = true;
            }
            else
            {
                this.comboBox4.Enabled = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.comboBox5.Enabled = true;
            }
            else
            {
                this.comboBox5.Enabled = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked)
            {
                this.comboBox6.Enabled = true;
            }
            else
            {
                this.comboBox6.Enabled = false;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.tabControl2.SelectedTab = this.tabPage5;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox10_Click(object sender, EventArgs e)
        {
            if (this.checkBox10.Checked)
            {
                this.dataGridView1.DataSource = _context.GetQuery("SELECT person.id AS 'ID', fio AS 'ПІП', phone AS 'Телефон', city AS 'Місто', vacancy.name AS 'Вакансія', target_price AS 'Очікувана ЗП, $', vacancy.price AS 'Оклад на позиції', priceK AS 'ЗПК', score AS 'K вмінь', s.sum_c AS 'Стаж, р.' FROM person LEFT JOIN (SELECT id_person, sum(Floor(DATEDIFF(date_end, date_start))/365) sum_c FROM experience GROUP BY experience.id_person) s ON person.id = s.id_person LEFT JOIN vacancy ON person.id_vacancy = vacancy.id INNER JOIN rating ON person.id = rating.id_person ORDER BY s.sum_c DESC ");
                drawRows();
            }
            else
            {
                mainTable();
                drawRows();
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Ви впевнені що хочете видалити користувача?", "Confirmation", MessageBoxButtons.YesNoCancel);

                int id_row = (int)this.dataGridView1.CurrentRow.Cells[0].Value;

                if (result == DialogResult.Yes)
                {
                    _context.GetQuery("DELETE FROM person WHERE id = '"+id_row+"'");
                    _context.GetQuery("DELETE FROM experience WHERE id_person = '"+id_row+"'");
                    _context.GetQuery("DELETE FROM quality WHERE id_person = '"+id_row+"'");
                    _context.GetQuery("DELETE FROM rating WHERE id_person = '"+id_row+"'");
                    mainTable();
                    drawRows();
                }
            }
            else
            {
                MessageBox.Show("Виберіть рядок для видалення");
            }
           
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void checkBox9_Click(object sender, EventArgs e)
        {
            if (this.checkBox9.Checked)
            {

                this.dataGridView1.DataSource = _context.GetQuery("SELECT person.id AS 'ID', fio AS 'ПІП', phone AS 'Телефон', city AS 'Місто', vacancy.name AS 'Вакансія', target_price AS 'Очікувана ЗП, $', vacancy.price AS 'Оклад на позиції', priceK AS 'ЗПК', score AS 'K вмінь', s.sum_c AS 'Стаж, р.' FROM person LEFT JOIN (SELECT id_person, sum(Floor(DATEDIFF(date_end, date_start))/365) sum_c FROM experience GROUP BY experience.id_person) s ON person.id = s.id_person LEFT JOIN vacancy ON person.id_vacancy = vacancy.id INNER JOIN rating ON person.id = rating.id_person ORDER BY rating.score DESC ");
                drawRows();
            }
            else
            {
                mainTable();
                drawRows();
            }
        }

        private void checkBox11_Click(object sender, EventArgs e)
        {
            if (this.checkBox11.Checked)
            {

                this.dataGridView1.DataSource = _context.GetQuery("SELECT person.id AS 'ID', fio AS 'ПІП', phone AS 'Телефон', city AS 'Місто', vacancy.name AS 'Вакансія', target_price AS 'Очікувана ЗП, $', vacancy.price AS 'Оклад на позиції', priceK AS 'ЗПК', score AS 'K вмінь', s.sum_c AS 'Стаж, р.' FROM person LEFT JOIN (SELECT id_person, sum(Floor(DATEDIFF(date_end, date_start))/365) sum_c FROM experience GROUP BY experience.id_person) s ON person.id = s.id_person LEFT JOIN vacancy ON person.id_vacancy = vacancy.id INNER JOIN rating ON person.id = rating.id_person ORDER BY rating.priceK DESC ");
                drawRows();
            }
            else
            {
                mainTable();
                drawRows();
            }
        }
    }
}
