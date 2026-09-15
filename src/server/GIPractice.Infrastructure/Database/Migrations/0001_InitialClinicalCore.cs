using FluentMigrator;

namespace GIPractice.Infrastructure.Database.Migrations;

[Migration(202609150001)]
public sealed class InitialClinicalCore : Migration
{
    public override void Up()
    {
        Create.Table("patients")
            .WithColumn("id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("first_name").AsString(200).NotNullable()
            .WithColumn("last_name").AsString(200).NotNullable()
            .WithColumn("fathers_name").AsString(200).Nullable()
            .WithColumn("birth_date").AsDate().Nullable()
            .WithColumn("gender").AsInt16().NotNullable();

        Create.Table("appointments")
            .WithColumn("id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("patient_id").AsString(36).NotNullable()
            .WithColumn("kind").AsInt16().NotNullable()
            .WithColumn("scheduled_start_utc").AsDateTime().NotNullable()
            .WithColumn("duration_minutes").AsInt32().NotNullable()
            .WithColumn("status").AsInt16().NotNullable()
            .WithColumn("is_urgent").AsBoolean().NotNullable()
            .WithColumn("notes").AsString().Nullable();

        Create.Table("encounters")
            .WithColumn("id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("patient_id").AsString(36).NotNullable()
            .WithColumn("kind").AsInt16().NotNullable()
            .WithColumn("started_at_utc").AsDateTime().NotNullable()
            .WithColumn("ended_at_utc").AsDateTime().Nullable()
            .WithColumn("status").AsInt16().NotNullable()
            .WithColumn("is_urgent").AsBoolean().NotNullable()
            .WithColumn("notes").AsString().Nullable();

        Create.Table("appointment_encounter_links")
            .WithColumn("appointment_id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("encounter_id").AsString(36).NotNullable();

        Create.Table("visits")
            .WithColumn("encounter_id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("kind").AsInt16().NotNullable();

        Create.Table("endoscopies")
            .WithColumn("encounter_id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("endoscopy_type").AsInt16().NotNullable()
            .WithColumn("report_document_json").AsString().Nullable();

        Create.Table("clinical_exams")
            .WithColumn("encounter_id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("has_serious_findings").AsBoolean().NotNullable()
            .WithColumn("clinical_notes").AsString().Nullable();

        Create.Table("infai_tests")
            .WithColumn("encounter_id").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("result").AsInt16().NotNullable()
            .WithColumn("patient_contacted").AsBoolean().NotNullable()
            .WithColumn("report_storage_key").AsString(1024).Nullable();

        Create.Table("practice_state")
            .WithColumn("id").AsInt32().NotNullable().PrimaryKey()
            .WithColumn("active_encounter_id").AsString(36).Nullable();

        Create.ForeignKey("fk_appointments_patient")
            .FromTable("appointments").ForeignColumn("patient_id")
            .ToTable("patients").PrimaryColumn("id");

        Create.ForeignKey("fk_encounters_patient")
            .FromTable("encounters").ForeignColumn("patient_id")
            .ToTable("patients").PrimaryColumn("id");

        Create.ForeignKey("fk_appointment_encounter_appointment")
            .FromTable("appointment_encounter_links").ForeignColumn("appointment_id")
            .ToTable("appointments").PrimaryColumn("id");

        Create.ForeignKey("fk_appointment_encounter_encounter")
            .FromTable("appointment_encounter_links").ForeignColumn("encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.ForeignKey("fk_visits_encounter")
            .FromTable("visits").ForeignColumn("encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.ForeignKey("fk_endoscopies_encounter")
            .FromTable("endoscopies").ForeignColumn("encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.ForeignKey("fk_clinical_exams_encounter")
            .FromTable("clinical_exams").ForeignColumn("encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.ForeignKey("fk_infai_tests_encounter")
            .FromTable("infai_tests").ForeignColumn("encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.ForeignKey("fk_practice_state_active_encounter")
            .FromTable("practice_state").ForeignColumn("active_encounter_id")
            .ToTable("encounters").PrimaryColumn("id");

        Create.Index("ux_appointment_encounter_links_encounter")
            .OnTable("appointment_encounter_links")
            .OnColumn("encounter_id").Ascending()
            .WithOptions().Unique();

        Create.Index("ix_appointments_schedule")
            .OnTable("appointments")
            .OnColumn("scheduled_start_utc").Ascending();

        Create.Index("ix_appointments_patient")
            .OnTable("appointments")
            .OnColumn("patient_id").Ascending();

        Create.Index("ix_encounters_started")
            .OnTable("encounters")
            .OnColumn("started_at_utc").Descending();

        Create.Index("ix_encounters_patient")
            .OnTable("encounters")
            .OnColumn("patient_id").Ascending();

        Insert.IntoTable("practice_state").Row(new { id = 1, active_encounter_id = (string?)null });
    }

    public override void Down()
    {
        Delete.Table("practice_state");
        Delete.Table("infai_tests");
        Delete.Table("clinical_exams");
        Delete.Table("endoscopies");
        Delete.Table("visits");
        Delete.Table("appointment_encounter_links");
        Delete.Table("encounters");
        Delete.Table("appointments");
        Delete.Table("patients");
    }
}
