using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DoAnLapTrinhWeb.Models
{
    // =========================
    // MODEL DANH MỤC CÔNG VIỆC
    // =========================
    public class JobCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }


    // =========================
    // MODEL CÔNG TY
    // =========================
    public class Company
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string CompanySize { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public bool IsVerified { get; set; }
    }


    // =========================
    // MODEL JOB
    // =========================
    public class Job
    {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public long PostedBy { get; set; }

        public int? CategoryId { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string Benefits { get; set; }

        public string EmploymentType { get; set; }

        public string Level { get; set; }

        public string WorkMode { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public int? SalaryMin { get; set; }

        public int? SalaryMax { get; set; }

        public bool SalaryNegotiable { get; set; }

        public string Currency { get; set; }

        public decimal MinExperienceYears { get; set; }

        public string MinEducation { get; set; }

        public short Headcount { get; set; }

        public string Status { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        // Các thông tin lấy thêm từ bảng Company / JobCategory
        public string CompanyName { get; set; }

        public string CompanyLogo { get; set; }

        public string CategoryName { get; set; }
    }


    // =========================
    // MODEL ỨNG VIÊN
    // =========================
    public class Candidate
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public long Id { get; set; }

        public long UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string City { get; set; }

        public string Headline { get; set; }

        public string Summary { get; set; }

        public decimal TotalExperienceYears { get; set; }

        public string HighestEducation { get; set; }

        public int? ExpectedSalary { get; set; }

        public bool OpenToWork { get; set; }
    }


    // =========================
    // MODEL SKILL
    // =========================
    public class Skill
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }
    }


    // =========================
    // MODEL APPLICATION
    // =========================
    public class Application
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public long CandidateId { get; set; }

        public long CvId { get; set; }

        public string CoverLetter { get; set; }

        public string Status { get; set; }

        public decimal? MatchScore { get; set; }

        public byte? RequiredMatched { get; set; }

        public byte? RequiredTotal { get; set; }

        public string RecruiterNote { get; set; }

        public DateTime AppliedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }


    // =========================
    // DATABASE
    // =========================
    public class Database
    {
        public List<User> dsUser = new List<User>();

        public List<Company> dsCompany =
            new List<Company>();

        public List<CompanyMember> dsCompanyMember =
            new List<CompanyMember>();

        public List<SkillCategory> dsSkillCategory =
            new List<SkillCategory>();

        public List<Skill> dsSkill =
            new List<Skill>();

        public List<SkillAlias> dsSkillAlias =
            new List<SkillAlias>();

        public List<Candidate> dsCandidate =
            new List<Candidate>();

        public List<CandidateSkill> dsCandidateSkill =
            new List<CandidateSkill>();

        public List<CandidateExperience> dsCandidateExperience =
            new List<CandidateExperience>();

        public List<CandidateEducation> dsCandidateEducation =
            new List<CandidateEducation>();

        public List<CV> dsCV =
            new List<CV>();

        public List<JobCategory> dsJobCategory =
            new List<JobCategory>();

        public List<Job> dsJob =
            new List<Job>();

        public List<JobSkill> dsJobSkill =
            new List<JobSkill>();

        public List<Application> dsApplication =
            new List<Application>();

        public List<ApplicationStatusHistory> dsApplicationStatusHistory =
            new List<ApplicationStatusHistory>();

        public List<SavedJob> dsSavedJob =
            new List<SavedJob>();

        // Chuỗi kết nối database
        string connStr =
            "Data Source=.;Initial Catalog=recruitment_db;Integrated Security=True;";


        public void GetUser()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM users",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                User u = new User();

                u.Id =
                    long.Parse(dr["id"].ToString());

                u.Email =
                    dr["email"].ToString();

                u.PasswordHash =
                    dr["password_hash"].ToString();

                u.Role =
                    dr["role"].ToString();

                u.Status =
                    dr["status"].ToString();

                if (dr["last_login_at"] != DBNull.Value)
                {
                    u.LastLoginAt =
                        DateTime.Parse(
                            dr["last_login_at"].ToString()
                        );
                }

                u.CreatedAt =
                    DateTime.Parse(
                        dr["created_at"].ToString()
                    );

                u.UpdatedAt =
                    DateTime.Parse(
                        dr["updated_at"].ToString()
                    );

                dsUser.Add(u);
            }
        }


        // =====================================================
        // 2. GET COMPANY
        // =====================================================

        public void GetCompany()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM companies",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Company c = new Company();

                c.Id =
                    long.Parse(dr["id"].ToString());

                c.Name =
                    dr["name"].ToString();

                c.TaxCode =
                    dr["tax_code"].ToString();

                c.LogoUrl =
                    dr["logo_url"].ToString();

                c.Website =
                    dr["website"].ToString();

                c.Industry =
                    dr["industry"].ToString();

                c.CompanySize =
                    dr["company_size"].ToString();

                c.Address =
                    dr["address"].ToString();

                c.City =
                    dr["city"].ToString();

                c.Description =
                    dr["description"].ToString();

                c.IsVerified =
                    bool.Parse(
                        dr["is_verified"].ToString()
                    );

                c.CreatedAt =
                        dr["created_at"].ToString();

                c.UpdatedAt =
                        dr["updated_at"].ToString();

                dsCompany.Add(c);
            }
        }


        // =====================================================
        // 3. GET COMPANY MEMBER
        // =====================================================

        public void GetCompanyMember()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM company_members",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                CompanyMember cm =
                    new CompanyMember();

                cm.CompanyId =
                    long.Parse(
                        dr["company_id"].ToString()
                    );

                cm.UserId =
                    long.Parse(
                        dr["user_id"].ToString()
                    );

                cm.FullName =
                    dr["full_name"].ToString();

                cm.Phone =
                    dr["phone"].ToString();

                cm.Position =
                    dr["position"].ToString();

                cm.MemberRole =
                    dr["member_role"].ToString();

                dsCompanyMember.Add(cm);
            }
        }


        // =====================================================
        // 4. GET SKILL CATEGORY
        // =====================================================

        public void GetSkillCategory()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM skill_categories",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                SkillCategory sc =
                    new SkillCategory();

                sc.Id =
                    int.Parse(
                        dr["id"].ToString()
                    );

                sc.Name =
                    dr["name"].ToString();

                dsSkillCategory.Add(sc);
            }
        }


        // =====================================================
        // 5. GET SKILL
        // =====================================================

        public void GetSkill()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM skills",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Skill s = new Skill();

                s.Id =
                    int.Parse(
                        dr["id"].ToString()
                    );

                if (dr["category_id"] != DBNull.Value)
                {
                    s.CategoryId =
                        int.Parse(
                            dr["category_id"].ToString()
                        );
                }

                s.Name =
                    dr["name"].ToString();

                s.Slug =
                    dr["slug"].ToString();

                dsSkill.Add(s);
            }
        }


        // =====================================================
        // 6. GET SKILL ALIAS
        // =====================================================

        public void GetSkillAlias()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM skill_aliases",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                SkillAlias sa =
                    new SkillAlias();

                sa.Id =
                    int.Parse(
                        dr["id"].ToString()
                    );

                sa.SkillId =
                    int.Parse(
                        dr["skill_id"].ToString()
                    );

                sa.Alias =
                    dr["alias"].ToString();

                dsSkillAlias.Add(sa);
            }
        }


        // =====================================================
        // 7. GET CANDIDATE
        // =====================================================

        public void GetCandidate()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM candidates",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Candidate c =
                    new Candidate();

                c.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                c.UserId =
                    long.Parse(
                        dr["user_id"].ToString()
                    );

                c.FullName =
                    dr["full_name"].ToString();

                c.Phone =
                    dr["phone"].ToString();

                if (dr["date_of_birth"] != DBNull.Value)
                {
                    c.DateOfBirth =
                        DateTime.Parse(
                            dr["date_of_birth"].ToString()
                        );
                }

                c.Gender =
                    dr["gender"].ToString();

                c.City =
                    dr["city"].ToString();

                c.Headline =
                    dr["headline"].ToString();

                c.Summary =
                    dr["summary"].ToString();

                c.TotalExperienceYears =
                    decimal.Parse(
                        dr["total_experience_years"].ToString()
                    );

                c.HighestEducation =
                    dr["highest_education"].ToString();

                if (dr["expected_salary"] != DBNull.Value)
                {
                    c.ExpectedSalary =
                        int.Parse(
                            dr["expected_salary"].ToString()
                        );
                }

                c.OpenToWork =
                    bool.Parse(
                        dr["open_to_work"].ToString()
                    );

                c.CreatedAt =
                        dr["created_at"].ToString();

                c.UpdatedAt =dr["updated_at"].ToString();

                dsCandidate.Add(c);
            }
        }


        // =====================================================
        // 8. GET CANDIDATE SKILL
        // =====================================================

        public void GetCandidateSkill()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM candidate_skills",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                CandidateSkill cs =
                    new CandidateSkill();

                cs.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                cs.SkillId =
                    int.Parse(
                        dr["skill_id"].ToString()
                    );

                cs.Proficiency =
                    byte.Parse(
                        dr["proficiency"].ToString()
                    );

                cs.YearsExperience =
                    decimal.Parse(
                        dr["years_experience"].ToString()
                    );

                dsCandidateSkill.Add(cs);
            }
        }


        // =====================================================
        // 9. GET CANDIDATE EXPERIENCE
        // =====================================================

        public void GetCandidateExperience()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM candidate_experiences",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                CandidateExperience ce =
                    new CandidateExperience();

                ce.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                ce.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                ce.CompanyName =
                    dr["company_name"].ToString();

                ce.JobTitle =
                    dr["job_title"].ToString();

                ce.StartDate =
                    DateTime.Parse(
                        dr["start_date"].ToString()
                    );

                if (dr["end_date"] != DBNull.Value)
                {
                    ce.EndDate =
                        DateTime.Parse(
                            dr["end_date"].ToString()
                        );
                }

                ce.Description =
                    dr["description"].ToString();

                dsCandidateExperience.Add(ce);
            }
        }


        // =====================================================
        // 10. GET CANDIDATE EDUCATION
        // =====================================================

        public void GetCandidateEducation()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM candidate_educations",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                CandidateEducation ce =
                    new CandidateEducation();

                ce.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                ce.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                ce.SchoolName =
                    dr["school_name"].ToString();

                ce.Major =
                    dr["major"].ToString();

                ce.Degree =
                    dr["degree"].ToString();

                if (dr["start_year"] != DBNull.Value)
                {
                    ce.StartYear =
                        short.Parse(
                            dr["start_year"].ToString()
                        );
                }

                if (dr["end_year"] != DBNull.Value)
                {
                    ce.EndYear =
                        short.Parse(
                            dr["end_year"].ToString()
                        );
                }

                dsCandidateEducation.Add(ce);
            }
        }


        // =====================================================
        // 11. GET CV
        // =====================================================

        public void GetCV()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM cvs",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                CV cv = new CV();

                cv.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                cv.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                cv.Title =
                    dr["title"].ToString();

                cv.FileUrl =
                    dr["file_url"].ToString();

                cv.ParsedText =
                    dr["parsed_text"].ToString();

                cv.IsDefault =
                    bool.Parse(
                        dr["is_default"].ToString()
                    );

                cv.UploadedAt =
                    DateTime.Parse(
                        dr["uploaded_at"].ToString()
                    );

                dsCV.Add(cv);
            }
        }


        // =====================================================
        // 12. GET JOB CATEGORY
        // =====================================================

        public void GetJobCategory()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM job_categories",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                JobCategory jc =
                    new JobCategory();

                jc.Id =
                    int.Parse(
                        dr["id"].ToString()
                    );

                jc.Name =
                    dr["name"].ToString();

                jc.Slug =
                    dr["slug"].ToString();

                dsJobCategory.Add(jc);
            }
        }


        // =====================================================
        // 13. GET JOB
        // =====================================================

        public void GetJob()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            string sql = @"
                SELECT
                    j.*,
                    c.name AS CompanyName,
                    c.logo_url AS CompanyLogo,
                    jc.name AS CategoryName
                FROM jobs j
                INNER JOIN companies c
                    ON j.company_id = c.id
                LEFT JOIN job_categories jc
                    ON j.category_id = jc.id
            ";

            SqlDataAdapter da =
                new SqlDataAdapter(
                    sql,
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Job j = new Job();

                j.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                j.CompanyId =
                    long.Parse(
                        dr["company_id"].ToString()
                    );

                j.PostedBy =
                    long.Parse(
                        dr["posted_by"].ToString()
                    );

                if (dr["category_id"] != DBNull.Value)
                {
                    j.CategoryId =
                        int.Parse(
                            dr["category_id"].ToString()
                        );
                }

                j.Title =
                    dr["title"].ToString();

                j.Slug =
                    dr["slug"].ToString();

                j.Description =
                    dr["description"].ToString();

                j.Requirements =
                    dr["requirements"].ToString();

                j.Benefits =
                    dr["benefits"].ToString();

                j.EmploymentType =
                    dr["employment_type"].ToString();

                j.Level =
                    dr["level"].ToString();

                j.WorkMode =
                    dr["work_mode"].ToString();

                j.City =
                    dr["city"].ToString();

                j.Address =
                    dr["address"].ToString();

                if (dr["salary_min"] != DBNull.Value)
                {
                    j.SalaryMin =
                        int.Parse(
                            dr["salary_min"].ToString()
                        );
                }

                if (dr["salary_max"] != DBNull.Value)
                {
                    j.SalaryMax =
                        int.Parse(
                            dr["salary_max"].ToString()
                        );
                }

                j.SalaryNegotiable =
                    bool.Parse(
                        dr["salary_negotiable"].ToString()
                    );

                j.Currency =
                    dr["currency"].ToString();

                j.MinExperienceYears =
                    decimal.Parse(
                        dr["min_experience_years"].ToString()
                    );

                j.MinEducation =
                    dr["min_education"].ToString();

                j.Headcount =
                    short.Parse(
                        dr["headcount"].ToString()
                    );

                j.Status =
                    dr["status"].ToString();

                if (dr["published_at"] != DBNull.Value)
                {
                    j.PublishedAt =
                        DateTime.Parse(
                            dr["published_at"].ToString()
                        );
                }

                if (dr["expires_at"] != DBNull.Value)
                {
                    j.ExpiresAt =
                        DateTime.Parse(
                            dr["expires_at"].ToString()
                        );
                }

                j.ViewCount =
                    int.Parse(
                        dr["view_count"].ToString()
                    );

                j.CreatedAt =
                    DateTime.Parse(
                        dr["created_at"].ToString()
                    );

                j.UpdatedAt =
                    DateTime.Parse(
                        dr["updated_at"].ToString()
                    );

                // Thông tin JOIN
                j.CompanyName =
                    dr["CompanyName"].ToString();

                j.CompanyLogo =
                    dr["CompanyLogo"].ToString();

                j.CategoryName =
                    dr["CategoryName"].ToString();

                dsJob.Add(j);
            }
        }


        // =====================================================
        // 14. GET JOB SKILL
        // =====================================================

        public void GetJobSkill()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM job_skills",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                JobSkill js =
                    new JobSkill();

                js.JobId =
                    long.Parse(
                        dr["job_id"].ToString()
                    );

                js.SkillId =
                    int.Parse(
                        dr["skill_id"].ToString()
                    );

                js.Importance =
                    dr["importance"].ToString();

                js.Weight =
                    byte.Parse(
                        dr["weight"].ToString()
                    );

                js.MinProficiency =
                    byte.Parse(
                        dr["min_proficiency"].ToString()
                    );

                js.MinYears =
                    decimal.Parse(
                        dr["min_years"].ToString()
                    );

                dsJobSkill.Add(js);
            }
        }


        // =====================================================
        // 15. GET APPLICATION
        // =====================================================

        public void GetApplication()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM applications",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Application a =
                    new Application();

                a.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                a.JobId =
                    long.Parse(
                        dr["job_id"].ToString()
                    );

                a.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                a.CvId =
                    long.Parse(
                        dr["cv_id"].ToString()
                    );

                a.CoverLetter =
                    dr["cover_letter"].ToString();

                a.Status =
                    dr["status"].ToString();

                if (dr["match_score"] != DBNull.Value)
                {
                    a.MatchScore =
                        decimal.Parse(
                            dr["match_score"].ToString()
                        );
                }

                if (dr["required_matched"] != DBNull.Value)
                {
                    a.RequiredMatched =
                        byte.Parse(
                            dr["required_matched"].ToString()
                        );
                }

                if (dr["required_total"] != DBNull.Value)
                {
                    a.RequiredTotal =
                        byte.Parse(
                            dr["required_total"].ToString()
                        );
                }

                a.RecruiterNote =
                    dr["recruiter_note"].ToString();

                a.AppliedAt =
                    DateTime.Parse(
                        dr["applied_at"].ToString()
                    );

                a.UpdatedAt =
                    DateTime.Parse(
                        dr["updated_at"].ToString()
                    );

                dsApplication.Add(a);
            }
        }


        // =====================================================
        // 16. GET APPLICATION STATUS HISTORY
        // =====================================================

        public void GetApplicationStatusHistory()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM application_status_history",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                ApplicationStatusHistory ash =
                    new ApplicationStatusHistory();

                ash.Id =
                    long.Parse(
                        dr["id"].ToString()
                    );

                ash.ApplicationId =
                    long.Parse(
                        dr["application_id"].ToString()
                    );

                ash.FromStatus =
                    dr["from_status"].ToString();

                ash.ToStatus =
                    dr["to_status"].ToString();

                if (dr["changed_by"] != DBNull.Value)
                {
                    ash.ChangedBy =
                        long.Parse(
                            dr["changed_by"].ToString()
                        );
                }

                ash.Note =
                    dr["note"].ToString();

                ash.ChangedAt =
                    DateTime.Parse(
                        dr["changed_at"].ToString()
                    );

                dsApplicationStatusHistory.Add(ash);
            }
        }


        // =====================================================
        // 17. GET SAVED JOB
        // =====================================================

        public void GetSavedJob()
        {
            SqlConnection conn =
                new SqlConnection(connStr);

            SqlDataAdapter da =
                new SqlDataAdapter(
                    "SELECT * FROM saved_jobs",
                    conn
                );

            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                SavedJob sj =
                    new SavedJob();

                sj.CandidateId =
                    long.Parse(
                        dr["candidate_id"].ToString()
                    );

                sj.JobId =
                    long.Parse(
                        dr["job_id"].ToString()
                    );

                sj.SavedAt =
                    DateTime.Parse(
                        dr["saved_at"].ToString()
                    );

                dsSavedJob.Add(sj);
            }
        }
        public Database()
        {
            GetUser();

            GetCompany();

            GetCompanyMember();

            GetSkillCategory();

            GetSkill();

            GetSkillAlias();

            GetCandidate();

            GetCandidateSkill();

            GetCandidateExperience();

            GetCandidateEducation();

            GetCV();

            GetJobCategory();

            GetJob();

            GetJobSkill();

            GetApplication();

            GetApplicationStatusHistory();

            GetSavedJob();
        }
    }
    public class User
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
    public class CompanyMember
    {
        public long CompanyId { get; set; }

        public long UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Position { get; set; }

        public string MemberRole { get; set; }
    }
    public class SkillCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class SkillAlias
    {
        public int Id { get; set; }

        public int SkillId { get; set; }

        public string Alias { get; set; }
    }
    public class CandidateSkill
    {
        public long CandidateId { get; set; }

        public int SkillId { get; set; }

        public byte Proficiency { get; set; }

        public decimal YearsExperience { get; set; }
    }
    public class CandidateExperience
    {
        public long Id { get; set; }

        public long CandidateId { get; set; }

        public string CompanyName { get; set; }

        public string JobTitle { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }
    }
    public class CandidateEducation
    {
        public long Id { get; set; }

        public long CandidateId { get; set; }

        public string SchoolName { get; set; }

        public string Major { get; set; }

        public string Degree { get; set; }

        public short? StartYear { get; set; }

        public short? EndYear { get; set; }
    }
    public class CV
    {
        public long Id { get; set; }

        public long CandidateId { get; set; }

        public string Title { get; set; }

        public string FileUrl { get; set; }

        public string ParsedText { get; set; }

        public bool IsDefault { get; set; }

        public DateTime UploadedAt { get; set; }
    }
    public class JobSkill
    {
        public long JobId { get; set; }

        public int SkillId { get; set; }

        public string Importance { get; set; }

        public byte Weight { get; set; }

        public byte MinProficiency { get; set; }

        public decimal MinYears { get; set; }
    }
    public class ApplicationStatusHistory
    {
        public long Id { get; set; }

        public long ApplicationId { get; set; }

        public string FromStatus { get; set; }

        public string ToStatus { get; set; }

        public long? ChangedBy { get; set; }

        public string Note { get; set; }

        public DateTime ChangedAt { get; set; }
    }
    public class SavedJob
    {
        public long CandidateId { get; set; }

        public long JobId { get; set; }

        public DateTime SavedAt { get; set; }
    }
}