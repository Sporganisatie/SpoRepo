using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SpoRE.Models.Settings;

namespace SpoRE.Infrastructure.Database;

public class DatabaseContext : DbContext
{
    private AppSettings _configuration;

    public DatabaseContext(IOptions<AppSettings> configuration)
    {
        _configuration = configuration.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.DbConnectionString);
        optionsBuilder.EnableSensitiveDataLogging(); // TODO only when localdev
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); // TODO only when localdev
    }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<AccountParticipation> AccountParticipations { get; set; }

    public DbSet<TeamSelection> TeamSelections { get; set; }

    public DbSet<AccountToken> AccountTokens { get; set; }

    public DbSet<Race> Races { get; set; }

    public DbSet<ResultsPoint> ResultsPoints { get; set; }

    public DbSet<Rider> Riders { get; set; }

    public DbSet<RiderParticipation> RiderParticipations { get; set; }

    public DbSet<Stage> Stages { get; set; }

    public DbSet<StageSelection> StageSelections { get; set; }

    public DbSet<StageSelectionRider> StageSelectionRiders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pg_catalog", "plv8")
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("citext")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("dict_int")
            .HasPostgresExtension("dict_xsyn")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("fuzzystrmatch")
            .HasPostgresExtension("hstore")
            .HasPostgresExtension("intarray")
            .HasPostgresExtension("ltree")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("pgrowlocks")
            .HasPostgresExtension("pgstattuple")
            .HasPostgresExtension("tablefunc")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp")
            .HasPostgresExtension("xml2");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("account_pkey");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "account_email_key").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Admin)
                .HasDefaultValueSql("false")
                .HasColumnName("admin");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasColumnName("password");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Verified).HasColumnName("verified");
        });

        modelBuilder.Entity<AccountParticipation>(entity =>
        {
            entity.HasKey(e => e.AccountParticipationId).HasName("account_participation_pkey");

            entity.ToTable("account_participation");

            entity.HasIndex(e => new { e.AccountId, e.RaceId, e.BudgetParticipation }, "account_participation_account_id_race_id_budgetparticipatio_key").IsUnique();

            entity.Property(e => e.AccountParticipationId).HasColumnName("account_participation_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BudgetParticipation)
                .HasDefaultValueSql("false")
                .HasColumnName("budgetparticipation");
            entity.Property(e => e.FinalScore)
                .HasDefaultValueSql("0")
                .HasColumnName("finalscore");
            entity.Property(e => e.RaceId).HasColumnName("race_id");

            // entity.HasOne(d => d.Account).WithMany(p => p.AccountParticipations)
            //     .HasForeignKey(d => d.AccountId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("account_participation_account_id_fkey");

            // entity.HasOne(d => d.Race).WithMany(p => p.AccountParticipations)
            //     .HasForeignKey(d => d.RaceId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("account_participation_race_id_fkey");
        });

        modelBuilder.Entity<TeamSelection>(entity =>
        {
            entity.HasKey(e => new { e.AccountParticipationId, e.RiderParticipationId }).HasName("team_selection_rider_pkey");

            entity.ToTable("team_selection_rider");

            entity.Property(e => e.AccountParticipationId).HasColumnName("account_participation_id");
            entity.Property(e => e.RiderParticipationId).HasColumnName("rider_participation_id");

            // entity.HasOne(d => d.AccountParticipation).WithMany(p => p.TeamSelections)
            //     .HasForeignKey(d => d.AccountParticipationId)
            //         .OnDelete(DeleteBehavior.ClientSetNull)
            //         .HasConstraintName("team_selection_rider_account_participation_id_fkey");


            // entity.HasOne(d => d.RiderParticipation).WithMany(p => p.TeamSelections)
            //     .HasForeignKey(d => d.RiderParticipationId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("team_selection_rider_rider_participation_id_fkey");
            // TODO IndexerProperty investigate
        });

        modelBuilder.Entity<AccountToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("account_token_pkey");

            entity.ToTable("account_token");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Expiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiry");
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("token");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("type");

            // entity.HasOne(d => d.Account).WithMany(p => p.AccountTokens)
            //     .HasForeignKey(d => d.AccountId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("account_token_account_id_fkey");
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(e => e.RaceId).HasName("race_pkey");

            entity.ToTable("race");

            entity.HasIndex(e => new { e.Name, e.Year }, "race_name_year_key").IsUnique();

            entity.Property(e => e.RaceId).HasColumnName("race_id");
            entity.Property(e => e.Budget)
                .HasDefaultValueSql("0")
                .HasColumnName("budget");
            entity.Property(e => e.Finished)
                .HasDefaultValueSql("false")
                .HasColumnName("finished");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<ResultsPoint>(entity =>
        {
            entity.HasKey(e => new { e.StageId, e.RiderParticipationId }).HasName("results_points_pkey");

            entity.ToTable("results_points");

            entity.Property(e => e.StageId).HasColumnName("stage_id");
            entity.Property(e => e.RiderParticipationId).HasColumnName("rider_participation_id");
            entity.HasOne(e => e.Stage)
                .WithMany()
                .HasForeignKey(e => e.StageId)
                .HasConstraintName("results_points_stage_id_fkey");

            entity.Property(e => e.StagePos)
                .HasDefaultValueSql("0")
                .HasColumnName("stagepos");
            entity.Property(e => e.StageResult).HasColumnName("stageresult");
            entity.Property(e => e.StageScore)
                .HasDefaultValueSql("0")
                .HasColumnName("stagescore");

            entity.OwnsOne(e => e.Gc, gc =>
            {
                gc.Property(p => p.Change)
                    .HasDefaultValueSql("''::text")
                    .HasColumnName("gcchange");
                gc.Property(p => p.Position)
                    .HasDefaultValueSql("0")
                    .HasColumnName("gcpos");
                gc.Property(p => p.Result)
                    .HasColumnName("gcresult");
                gc.Property(p => p.Score)
                    .HasDefaultValueSql("0")
                    .HasColumnName("gcscore");
            });

            entity.OwnsOne(e => e.Points, points =>
            {
                points.Property(p => p.Change)
                    .HasDefaultValueSql("''::text")
                    .HasColumnName("pointschange");
                points.Property(p => p.Position)
                    .HasDefaultValueSql("0")
                    .HasColumnName("pointspos");
                points.Property(p => p.Result)
                    .HasColumnName("pointsresult");
                points.Property(p => p.Score)
                    .HasDefaultValueSql("0")
                    .HasColumnName("pointsscore");
            });

            entity.OwnsOne(e => e.Kom, kom =>
            {
                kom.Property(p => p.Change)
                    .HasDefaultValueSql("''::text")
                    .HasColumnName("komchange");
                kom.Property(p => p.Position)
                    .HasDefaultValueSql("0")
                    .HasColumnName("kompos");
                kom.Property(p => p.Result)
                    .HasColumnName("komresult");
                kom.Property(p => p.Score)
                    .HasDefaultValueSql("0")
                    .HasColumnName("komscore");
            });

            entity.OwnsOne(e => e.Youth, youth =>
            {
                youth.Property(p => p.Change)
                    .HasDefaultValueSql("''::text")
                    .HasColumnName("yocchange");
                youth.Property(p => p.Position)
                    .HasDefaultValueSql("0")
                    .HasColumnName("yocpos");
                youth.Property(p => p.Result)
                    .HasColumnName("yocresult");
                youth.Property(p => p.Score)
                    .HasDefaultValueSql("0")
                    .HasColumnName("yocscore");
            });

            entity.Property(e => e.Teamscore)
                .HasDefaultValueSql("0")
                .HasColumnName("teamscore");

            entity.Property(e => e.Totalscore)
                .HasDefaultValueSql("0")
                .HasColumnName("totalscore");
        });


        modelBuilder.Entity<Rider>(entity =>
        {
            entity.HasKey(e => e.RiderId).HasName("rider_pkey");

            entity.ToTable("rider");

            entity.HasIndex(e => e.PcsId, "rider_pcs_id_key").IsUnique();

            entity.Property(e => e.RiderId).HasColumnName("rider_id");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.Firstname)
                .IsRequired()
                .HasColumnName("firstname");
            entity.Property(e => e.Initials)
                .IsRequired()
                .HasColumnName("initials");
            entity.Property(e => e.Lastname)
                .IsRequired()
                .HasColumnName("lastname");
            entity.Property(e => e.PcsId)
                .IsRequired()
                .HasColumnName("pcs_id");
        });

        modelBuilder.Entity<RiderParticipation>(entity =>
        {
            entity.HasKey(e => e.RiderParticipationId).HasName("rider_participation_pkey");

            entity.ToTable("rider_participation");

            entity.HasIndex(e => new { e.RaceId, e.RiderId }, "rider_participation_race_id_rider_id_key").IsUnique();

            entity.Property(e => e.RiderParticipationId).HasColumnName("rider_participation_id");
            entity.Property(e => e.Climb).HasColumnName("climb");
            entity.Property(e => e.Dnf)
                .HasDefaultValueSql("false")
                .HasColumnName("dnf");
            entity.Property(e => e.Gc).HasColumnName("gc");
            entity.Property(e => e.Price)
                .HasDefaultValueSql("6666666")
                .HasColumnName("price");
            entity.Property(e => e.Punch).HasColumnName("punch");
            entity.Property(e => e.RaceId).HasColumnName("race_id");
            entity.Property(e => e.RiderId).HasColumnName("rider_id");
            entity.Property(e => e.Sprint).HasColumnName("sprint");
            entity.Property(e => e.Team)
                .IsRequired()
                .HasColumnName("team");
            entity.Property(e => e.Tt).HasColumnName("tt");

            // entity.HasOne(d => d.Race).WithMany(p => p.RiderParticipations)
            //     .HasForeignKey(d => d.RaceId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("rider_participation_race_id_fkey");

            // entity.HasOne(d => d.Rider).WithMany(p => p.RiderParticipations) TODO fix recursion error
            //     .HasForeignKey(d => d.RiderId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("rider_participation_rider_id_fkey");

            // entity.HasMany(d => d.StageSelectionsNavigation).WithMany(p => p.RiderParticipations)
            //     .UsingEntity<Dictionary<string, object>>(
            //         "StageSelectionRider", // TODO define stageselectionrider table like teamselection
            //         r => r.HasOne<StageSelection>().WithMany()
            //             .HasForeignKey("StageSelectionId")
            //             .OnDelete(DeleteBehavior.ClientSetNull)
            //             .HasConstraintName("stage_selection_rider_stage_selection_id_fkey"),
            //         l => l.HasOne<RiderParticipation>().WithMany()
            //             .HasForeignKey("RiderParticipationId")
            //             .OnDelete(DeleteBehavior.ClientSetNull)
            //             .HasConstraintName("stage_selection_rider_rider_participation_id_fkey"),
            //         j =>
            //         {
            //             j.HasKey("RiderParticipationId", "StageSelectionId").HasName("stage_selection_rider_pkey");
            //             j.ToTable("stage_selection_rider");
            //             j.IndexerProperty<int>("RiderParticipationId").HasColumnName("rider_participation_id");
            //             j.IndexerProperty<int>("StageSelectionId").HasColumnName("stage_selection_id");
            //         });
        });

        modelBuilder.Entity<Stage>(entity =>
        {
            entity.HasKey(e => e.StageId).HasName("stage_pkey");

            entity.ToTable("stage");

            entity.HasIndex(e => new { e.RaceId, e.Stagenr }, "stage_race_id_stagenr_key").IsUnique();

            entity.Property(e => e.StageId).HasColumnName("stage_id");
            entity.Property(e => e.Complete)
                .HasDefaultValueSql("false")
                .HasColumnName("complete");
            entity.Property(e => e.Finished)
                .HasDefaultValueSql("false")
                .HasColumnName("finished");
            entity.Property(e => e.RaceId).HasColumnName("race_id");
            entity.Property(e => e.Stagenr).HasColumnName("stagenr");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (StageType)Enum.Parse(typeof(StageType), v))
                .HasDefaultValue(StageType.REG)
                .HasColumnName("type");
            entity.Property(e => e.Weight)
                .HasDefaultValueSql("1")
                .HasColumnName("weight");

            // entity.HasOne(d => d.Race).WithMany(p => p.Stages)
            //     .HasForeignKey(d => d.RaceId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("stage_race_id_fkey");
        });

        modelBuilder.Entity<StageSelection>(entity =>
        {
            entity.HasKey(e => e.StageSelectionId).HasName("stage_selection_pkey");

            entity.ToTable("stage_selection");

            entity.HasIndex(e => new { e.AccountParticipationId, e.StageId }, "stage_selection_account_participation_id_stage_id_key").IsUnique();

            entity.Property(e => e.StageSelectionId).HasColumnName("stage_selection_id");
            entity.Property(e => e.AccountParticipationId).HasColumnName("account_participation_id");
            entity.Property(e => e.KopmanId).HasColumnName("kopman_id");
            entity.Property(e => e.StageId).HasColumnName("stage_id");
            entity.Property(e => e.StageScore)
                .HasDefaultValueSql("0")
                .HasColumnName("stagescore");
            entity.Property(e => e.TotalScore)
                .HasDefaultValueSql("0")
                .HasColumnName("totalscore");

            // entity.HasOne(d => d.AccountParticipation).WithMany(p => p.StageSelections)
            //     .HasForeignKey(d => d.AccountParticipationId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("stage_selection_account_participation_id_fkey");

            // entity.HasOne(d => d.Kopman).WithMany(p => p.StageSelections)
            //     .HasForeignKey(d => d.KopmanId)
            //     .HasConstraintName("stage_selection_kopman_id_fkey");

            // entity.HasOne(d => d.Stage).WithMany(p => p.StageSelections)
            //     .HasForeignKey(d => d.StageId)
            //     .OnDelete(DeleteBehavior.ClientSetNull)
            //     .HasConstraintName("stage_selection_stage_id_fkey");
        });

        modelBuilder.Entity<StageSelectionRider>(entity =>
        {
            entity.HasKey(e => new { e.StageSelectionId, e.RiderParticipationId }).HasName("stage_selection_rider_pkey");

            entity.ToTable("stage_selection_rider");

            entity.Property(e => e.StageSelectionId).HasColumnName("stage_selection_id");
            entity.Property(e => e.RiderParticipationId).HasColumnName("rider_participation_id");
        });
    }
}
