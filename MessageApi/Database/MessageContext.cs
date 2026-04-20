using MessageApi.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration;

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

        public MessageContext()
        {
        }

        public MessageContext(DbContextOptions<MessageContext> options) : base(options)
        {
        }

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

            // Seed initial accounts so the database is initialized with default users
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Username = "archi",
                    Password = "AQAAAAIAAYagAAAAEINoKBPVubTytHlU50RzwhBqlNLoL19huAKwccvxEUOQaYE8G+inmgJoCT7bpTxEwA==",
                    Name = "Archibaldo",
                    Status = "Ready to chat",
                    Role = "User",
                    ImageUrl = null
                },
                new Account
                {
                    Username = "moriarty",
                    Password = "AQAAAAIAAYagAAAAEIOYqTqM5ZBa8nGF6q+lNS6v2qwXBAafjIn0ZedetP546tI1GqLT6mD5A/rqRHssAA==",
                    Name = "Moriarty",
                    Status = "Ready to chat",
                    Role = "User",
                    ImageUrl = null
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false, reloadOnChange: false);
            IConfigurationRoot config = builder.Build();

            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql(config.GetConnectionString("DefaultConnection"));
        }
    }
}