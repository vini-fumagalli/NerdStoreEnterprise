use master
go

create database NerdStoreEnterpriseDB
go

USE NerdStoreEnterpriseDB
GO

SET QUOTED_IDENTIFIER ON;
GO
       
create sequence dbo.MinhaSequencia
    as int
    start with 1000
go

create table dbo.AspNetRoles
(
    Id               int identity
        constraint PK_AspNetRoles
            primary key,
    Name             nvarchar(256),
    NormalizedName   nvarchar(256),
    ConcurrencyStamp nvarchar(max)
)
    go

create table dbo.AspNetRoleClaims
(
    Id         int identity
        constraint PK_AspNetRoleClaims
            primary key,
    RoleId     int not null
        constraint FK_AspNetRoleClaims_AspNetRoles_RoleId
            references dbo.AspNetRoles
            on delete cascade,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max)
)
    go

create index IX_AspNetRoleClaims_RoleId
    on dbo.AspNetRoleClaims (RoleId)
    go

create unique index RoleNameIndex
    on dbo.AspNetRoles (NormalizedName)
    where [NormalizedName] IS NOT NULL
go

create table dbo.AspNetUsers
(
    Id                   int identity
        constraint PK_AspNetUsers
            primary key,
    UserName             nvarchar(256),
    NormalizedUserName   nvarchar(256),
    Email                nvarchar(256),
    NormalizedEmail      nvarchar(256),
    EmailConfirmed       bit not null,
    PasswordHash         nvarchar(max),
    SecurityStamp        nvarchar(max),
    ConcurrencyStamp     nvarchar(max),
    PhoneNumber          nvarchar(max),
    PhoneNumberConfirmed bit not null,
    TwoFactorEnabled     bit not null,
    LockoutEnd           datetimeoffset,
    LockoutEnabled       bit not null,
    AccessFailedCount    int not null
)
    go

create table dbo.AspNetUserClaims
(
    Id         int identity
        constraint PK_AspNetUserClaims
            primary key,
    UserId     int not null
        constraint FK_AspNetUserClaims_AspNetUsers_UserId
            references dbo.AspNetUsers
            on delete cascade,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max)
)
    go

create index IX_AspNetUserClaims_UserId
    on dbo.AspNetUserClaims (UserId)
    go

create table dbo.AspNetUserLogins
(
    LoginProvider       nvarchar(128) not null,
    ProviderKey         nvarchar(128) not null,
    ProviderDisplayName nvarchar(max),
    UserId              int           not null
        constraint FK_AspNetUserLogins_AspNetUsers_UserId
            references dbo.AspNetUsers
            on delete cascade,
    constraint PK_AspNetUserLogins
        primary key (LoginProvider, ProviderKey)
)
    go

create index IX_AspNetUserLogins_UserId
    on dbo.AspNetUserLogins (UserId)
    go

create table dbo.AspNetUserRoles
(
    UserId int not null
        constraint FK_AspNetUserRoles_AspNetUsers_UserId
            references dbo.AspNetUsers
            on delete cascade,
    RoleId int not null
        constraint FK_AspNetUserRoles_AspNetRoles_RoleId
            references dbo.AspNetRoles
            on delete cascade,
    constraint PK_AspNetUserRoles
        primary key (UserId, RoleId)
)
    go

create index IX_AspNetUserRoles_RoleId
    on dbo.AspNetUserRoles (RoleId)
    go

create table dbo.AspNetUserTokens
(
    UserId        int           not null
        constraint FK_AspNetUserTokens_AspNetUsers_UserId
            references dbo.AspNetUsers
            on delete cascade,
    LoginProvider nvarchar(128) not null,
    Name          nvarchar(128) not null,
    Value         nvarchar(max),
    constraint PK_AspNetUserTokens
        primary key (UserId, LoginProvider, Name)
)
    go

create index EmailIndex
    on dbo.AspNetUsers (NormalizedEmail)
    go

create unique index UserNameIndex
    on dbo.AspNetUsers (NormalizedUserName)
    where [NormalizedUserName] IS NOT NULL
go

create table dbo.CarrinhoCliente
(
    Id               int identity
        constraint PK_CarrinhoCliente
            primary key,
    ClienteId        int                                      not null,
    ValorTotal       decimal(18, 2)                           not null,
    Desconto         decimal(18, 2) default 0.0               not null,
    Percentual       decimal(18, 2),
    TipoDesconto     int            default 0                 not null,
    ValorDesconto    decimal(18, 2),
    VoucherCodigo    varchar(50)    default ''                not null,
    VoucherUtilizado bit            default CONVERT([bit], 0) not null
)
    go

create index IDX_Cliente
    on dbo.CarrinhoCliente (ClienteId)
    go

create table dbo.CarrinhoItens
(
    Id         int identity
        constraint PK_CarrinhoItens
            primary key,
    ProdutoId  int            not null,
    Nome       varchar(100)   not null,
    Quantidade int            not null,
    Valor      decimal(18, 2) not null,
    Imagem     varchar(100)   not null,
    CarrinhoId int            not null
        constraint FK_CarrinhoItens_CarrinhoCliente_CarrinhoId
            references dbo.CarrinhoCliente
            on delete cascade
)
    go

create index IX_CarrinhoItens_CarrinhoId
    on dbo.CarrinhoItens (CarrinhoId)
    go

create table dbo.Clientes
(
    Id       int          not null
        constraint PK_Clientes
            primary key,
    Nome     varchar(200) not null,
    Email    varchar(254) not null,
    Cpf      varchar(11)  not null,
    Excluido bit          not null
)
    go

create table dbo.CodAut
(
    Email              varchar(50) not null
        primary key,
    CodigoAutenticacao varchar(6)  not null
)
    go

create table dbo.Enderecos
(
    Id          int identity
        constraint PK_Enderecos
            primary key,
    Logradouro  varchar(200) not null,
    Numero      varchar(50)  not null,
    Complemento varchar(250) not null,
    Bairro      varchar(100) not null,
    Cep         varchar(20)  not null,
    Cidade      varchar(100) not null,
    Estado      varchar(50)  not null,
    ClienteId   int          not null
        constraint FK_Enderecos_Clientes_ClienteId
            references dbo.Clientes
)
    go

create unique index IX_Enderecos_ClienteId
    on dbo.Enderecos (ClienteId)
    go

create table dbo.Produtos
(
    Id                int identity
        constraint PK_Produtos
            primary key,
    Nome              varchar(250)   not null,
    Descricao         varchar(500)   not null,
    Ativo             bit            not null,
    Valor             decimal(18, 2) not null,
    DataCadastro      datetime2      not null,
    Imagem            varchar(250)   not null,
    QuantidadeEstoque int            not null
)
    go

create table dbo.RefreshTokens
(
    UsuarioId    int          not null
        primary key
        constraint FK_RefreshTokens_UsuarioId
            references dbo.AspNetUsers,
    RefreshToken varchar(250) not null,
    ValidoAte    datetime     not null
)
    go

create table dbo.Vouchers
(
    Id             int identity
        constraint PK_Vouchers
            primary key,
    Codigo         varchar(100) not null,
    Percentual     decimal(18, 2),
    ValorDesconto  decimal(18, 2),
    Quantidade     int          not null,
    TipoDesconto   int          not null,
    DataCriacao    datetime2    not null,
    DataUtilizacao datetime2,
    DataValidade   datetime2    not null,
    Ativo          bit          not null,
    Utilizado      bit          not null
)
    go

create table dbo.Pedidos
(
    Id               int identity
        constraint PK_Pedidos
            primary key,
    Codigo           int default NEXT VALUE FOR [MinhaSequencia] not null,
    ClienteId        int                                         not null,
    VoucherId        int
        constraint FK_Pedidos_Vouchers_VoucherId
            references dbo.Vouchers,
    VoucherUtilizado bit                                         not null,
    Desconto         decimal(18, 2)                              not null,
    ValorTotal       decimal(18, 2)                              not null,
    DataCadastro     datetime2                                   not null,
    PedidoStatus     int                                         not null,
    Logradouro       varchar(100)                                not null,
    Numero           varchar(100)                                not null,
    Complemento      varchar(100)                                not null,
    Bairro           varchar(100)                                not null,
    Cep              varchar(100)                                not null,
    Cidade           varchar(100)                                not null,
    Estado           varchar(100)                                not null
)
    go

create table dbo.PedidoItems
(
    Id            int identity
        constraint PK_PedidoItems
            primary key,
    PedidoId      int            not null
        constraint FK_PedidoItems_Pedidos_PedidoId
            references dbo.Pedidos,
    ProdutoId     int            not null,
    ProdutoNome   varchar(250)   not null,
    Quantidade    int            not null,
    ValorUnitario decimal(18, 2) not null,
    ProdutoImagem varchar(100)   not null
)
    go

create index IX_PedidoItems_PedidoId
    on dbo.PedidoItems (PedidoId)
    go

create index IX_Pedidos_VoucherId
    on dbo.Pedidos (VoucherId)
    go

create table dbo.__EFMigrationsHistory
(
    MigrationId    nvarchar(150) not null
        constraint PK___EFMigrationsHistory
            primary key,
    ProductVersion nvarchar(32)  not null
)
    go

INSERT INTO dbo.Produtos (Nome, Descricao, Ativo, Valor, DataCadastro, Imagem, QuantidadeEstoque) VALUES
('Camiseta 4 Head', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', '4head.webp', 5),
('Camiseta 4 Head Branca', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Branca 4head.webp', 5),
('Camiseta Tiltado', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'tiltado.webp', 10),
('Camiseta Tiltado Branca', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Branco Tiltado.webp', 10),
('Camiseta Heisenberg', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Heisenberg.webp', 10),
('Camiseta Kappa', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Kappa.webp', 10),
('Camiseta MacGyver', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'MacGyver.webp', 10),
('Camiseta Maestria', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Maestria.webp', 10),
('Camiseta Code Life Preta', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'camiseta2.jpg', 10),
('Camiseta My Yoda', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'My Yoda.webp', 10),
('Camiseta Pato', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Pato.webp', 10),
('Camiseta Xavier School', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Xaviers School.webp', 10),
('Camiseta Yoda', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Yoda.webp', 10),
('Camiseta Quack', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Quack.webp', 10),
('Camiseta Rick And Morty 2', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Rick And Morty Captured.webp', 10),
('Camiseta Rick And Morty', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Rick And Morty.webp', 5),
('Camiseta Say My Name', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Say My Name.webp', 10),
('Camiseta Support', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'support.webp', 10),
('Camiseta Try Hard', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 50.00, '2019-07-19T00:00:00', 'Tryhard.webp', 10),
('Caneca Joker Wanted', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-joker Wanted.jpg', 10),
('Caneca Joker', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Joker.jpg', 10),
('Caneca Nightmare', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Nightmare.jpg', 10),
('Caneca Ozob', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Ozob.webp', 10),
('Caneca Rick and Morty', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Rick and Morty.jpg', 5),
('Caneca Wonder Woman', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Wonder Woman.jpg', 10),
('Caneca No Coffee No Code', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca4.jpg', 10),
('Caneca Batman', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca1--batman.jpg', 5),
('Caneca Vegeta', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca1-Vegeta.jpg', 10),
('Caneca Batman Preta', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Batman.jpg', 8),
('Caneca Big Bang Theory', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-bbt.webp', 0),
('Caneca Cogumelo', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-cogumelo.webp', 10),
('Caneca Geeks', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-Geeks.jpg', 10),
('Caneca Ironman', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 50.00, '2019-07-19T00:00:00', 'caneca-ironman.jpg', 10),
('Camiseta Debugar Preta', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 75.00, '2019-07-19T00:00:00', 'camiseta4.jpg', 10),
('Camiseta Code Life Cinza', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 99.00, '2019-07-19T00:00:00', 'camiseta3.jpg', 3),
('Caneca Star Bugs Coffee', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 20.00, '2019-07-19T00:00:00', 'caneca1.jpg', 10),
('Caneca Programmer Code', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 15.00, '2019-07-19T00:00:00', 'caneca2.jpg', 10),
('Camiseta Software Developer', 'Camiseta 100% algodão, resistente a lavagens e altas temperaturas.', 1, 100.00, '2019-07-19T00:00:00', 'camiseta1.jpg', 10),
('Caneca Turn Coffee in Code', 'Caneca de porcelana com impressão térmica de alta resistência.', 1, 20.00, '2019-07-19T00:00:00', 'caneca3.jpg', 10)
;