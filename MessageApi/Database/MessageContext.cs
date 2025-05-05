using MessageApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageApi.Database
{
    public class MessageContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<AccountGroup> AccountGroup { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<AccountMessage> AccountMessage { get; set; }
        public DbSet<GroupMessage> GroupMessage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasOne<Account>(c => c.Sender)
                .WithMany(a => a.ChatsSent)
                .HasForeignKey(c => c.SenderUsername);

            modelBuilder.Entity<Chat>()
                .HasOne<Account>(c => c.Receiver)
                .WithMany(a => a.ChatsReceived)
                .HasForeignKey(c => c.ReceiverUsername);

            modelBuilder.Entity<Chat>()
                .HasOne<Message>(m => m.LastMessage)
                .WithOne(c => c.Chat)
                .HasForeignKey<Chat>(c => c.LastMessageId);

            modelBuilder.Entity<AccountMessage>().HasKey(sc => new { sc.AccountUsername, sc.MessageId });

            modelBuilder.Entity<AccountMessage>()
                .HasOne<Account>(sc => sc.Account)
                .WithMany(a => a.AccountMessages)
                .HasForeignKey(sc => sc.AccountUsername);

            modelBuilder.Entity<AccountMessage>()
                .HasOne<Message>(sc => sc.Message)
                .WithMany(a => a.AccountMessages)
                .HasForeignKey(sc => sc.MessageId);

            modelBuilder.Entity<AccountGroup>().HasKey(sc => new { sc.AccountUsername, sc.GroupId });

            modelBuilder.Entity<AccountGroup>()
                .HasOne<Account>(sc => sc.Account)
                .WithMany(a => a.AccountGroups)
                .HasForeignKey(sc => sc.AccountUsername);

            modelBuilder.Entity<AccountGroup>()
                .HasOne<Groups>(sc => sc.Group)
                .WithMany(a => a.GroupAccounts)
                .HasForeignKey(sc => sc.GroupId);

            modelBuilder.Entity<GroupMessage>().HasKey(sc => new { sc.GroupId, sc.MessageId });

            modelBuilder.Entity<GroupMessage>()
                .HasOne<Groups>(sc => sc.Group)
                .WithMany(a => a.GroupMessages)
                .HasForeignKey(sc => sc.GroupId);

            modelBuilder.Entity<GroupMessage>()
                .HasOne<Message>(sc => sc.Message)
                .WithMany(a => a.GroupMessages)
                .HasForeignKey(sc => sc.MessageId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json");
            IConfigurationRoot config = builder.Build();

            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(config["ConnectionString"]);
        }
    }
}