/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2008                    */
/* Created on:     02/02/2015 4:28:31 a. m.                     */
/*==============================================================*/


if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BLOQUE') and o.name = 'FK_BLOQUE_TIENE_PROYECTO')
alter table BLOQUE
   drop constraint FK_BLOQUE_TIENE_PROYECTO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('BLOQUE') and o.name = 'FK_PROYECTO_CONTENEDOR')
alter table BLOQUE
   drop constraint FK_PROYECTO_CONTENEDOR
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CONFIGURACIONUNIDADMUESTRAL') and o.name = 'FK_PROYECTO_TIENE_CONFIGURACION_UNIDADMUESTRAL')
alter table CONFIGURACIONUNIDADMUESTRAL
   drop constraint FK_PROYECTO_TIENE_CONFIGURACION_UNIDADMUESTRAL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('DOMINIOVARIABLE') and o.name = 'FK_DOMINIO_APLICA_VARIABLE')
alter table DOMINIOVARIABLE
   drop constraint FK_DOMINIO_APLICA_VARIABLE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('DOMINIOVARIABLE') and o.name = 'FK_VARIABLE_TIENE_DOMINIO')
alter table DOMINIOVARIABLE
   drop constraint FK_VARIABLE_TIENE_DOMINIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ESPECIE') and o.name = 'FK_ESPECIE_PERTENECE_GRUPOCOMERCIAL')
alter table ESPECIE
   drop constraint FK_ESPECIE_PERTENECE_GRUPOCOMERCIAL
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULA') and o.name = 'FK_FORMULA_ES_DE_TIPO_FORMULA')
alter table FORMULA
   drop constraint FK_FORMULA_ES_DE_TIPO_FORMULA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULAPROYECTO') and o.name = 'FK_FORMULA_SE_APLICA_A_PROYECTOS')
alter table FORMULAPROYECTO
   drop constraint FK_FORMULA_SE_APLICA_A_PROYECTOS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULAPROYECTO') and o.name = 'FK_PROYECTO_TIENE_FORMULA')
alter table FORMULAPROYECTO
   drop constraint FK_PROYECTO_TIENE_FORMULA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULARIO') and o.name = 'FK_FORMULARIO_TIENE_ESTRATO')
alter table FORMULARIO
   drop constraint FK_FORMULARIO_TIENE_ESTRATO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULARIO') and o.name = 'FK_PARCELA_TIENE_FORMULARIO')
alter table FORMULARIO
   drop constraint FK_PARCELA_TIENE_FORMULARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULARIO') and o.name = 'FK_USUARIO_CREA_FOMULARIO')
alter table FORMULARIO
   drop constraint FK_USUARIO_CREA_FOMULARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULAVARIABLE') and o.name = 'FK_FORMULA_TIENE_VARIABLE')
alter table FORMULAVARIABLE
   drop constraint FK_FORMULA_TIENE_VARIABLE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('FORMULAVARIABLE') and o.name = 'FK_VARIABLE_SE_APLICA_FORMULA')
alter table FORMULAVARIABLE
   drop constraint FK_VARIABLE_SE_APLICA_FORMULA
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('IMAGEN') and o.name = 'FK_ESPECIE_TIENE_IMAGEN')
alter table IMAGEN
   drop constraint FK_ESPECIE_TIENE_IMAGEN
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIO') and o.name = 'FK_FORMULARIO_CREA_LINEAINEVNTARIO')
alter table LINEAINVENTARIO
   drop constraint FK_FORMULARIO_CREA_LINEAINEVNTARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIO') and o.name = 'FK_LINEA_INVENTARIO_ES_DE_TIPOLINEAINV')
alter table LINEAINVENTARIO
   drop constraint FK_LINEA_INVENTARIO_ES_DE_TIPOLINEAINV
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIOESPECIE') and o.name = 'FK_ESPECIE_PERTENECE_LINEAINVENTARIO')
alter table LINEAINVENTARIOESPECIE
   drop constraint FK_ESPECIE_PERTENECE_LINEAINVENTARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIOESPECIE') and o.name = 'FK_LINEA_INVENTARIO_TIENE_ESPECIE')
alter table LINEAINVENTARIOESPECIE
   drop constraint FK_LINEA_INVENTARIO_TIENE_ESPECIE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIOVARIABLE') and o.name = 'FK_LINEAINVENTARIO_TIENE_VARIABLES')
alter table LINEAINVENTARIOVARIABLE
   drop constraint FK_LINEAINVENTARIO_TIENE_VARIABLES
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LINEAINVENTARIOVARIABLE') and o.name = 'FK_VARIABLES_SE_APLICA_LINEAINVENTARIO')
alter table LINEAINVENTARIOVARIABLE
   drop constraint FK_VARIABLES_SE_APLICA_LINEAINVENTARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODECOSTOS') and o.name = 'FK_LISTADODECOSTOS_TIENE_COSTO')
alter table LISTADODECOSTOS
   drop constraint FK_LISTADODECOSTOS_TIENE_COSTO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODECOSTOS') and o.name = 'FK_PROYECTO_TIENE_LISTADODECOSTOS')
alter table LISTADODECOSTOS
   drop constraint FK_PROYECTO_TIENE_LISTADODECOSTOS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODEESPECIES') and o.name = 'FK_LISTADODEESPECIES_TIENE_ESPECIE')
alter table LISTADODEESPECIES
   drop constraint FK_LISTADODEESPECIES_TIENE_ESPECIE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODEESPECIES') and o.name = 'FK_PROYECTO_TIENE_LISTADODEESPECIE')
alter table LISTADODEESPECIES
   drop constraint FK_PROYECTO_TIENE_LISTADODEESPECIE
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODEESTRATOS') and o.name = 'FK_LISTADEESTRATOS_TIENE_ESTRATOS')
alter table LISTADODEESTRATOS
   drop constraint FK_LISTADEESTRATOS_TIENE_ESTRATOS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LISTADODEESTRATOS') and o.name = 'FK_PROYECTO_TIENE_LISTADODEESTRATOS')
alter table LISTADODEESTRATOS
   drop constraint FK_PROYECTO_TIENE_LISTADODEESTRATOS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LOCALIDAD') and o.name = 'FK_LOCALIDAD_ESTA_CONTENIDA_LOCALIDAD')
alter table LOCALIDAD
   drop constraint FK_LOCALIDAD_ESTA_CONTENIDA_LOCALIDAD
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LOCALIDADPROYECTO') and o.name = 'FK_LOCALIDAD_ESTA_ASOCIADA_PROYECTO')
alter table LOCALIDADPROYECTO
   drop constraint FK_LOCALIDAD_ESTA_ASOCIADA_PROYECTO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('LOCALIDADPROYECTO') and o.name = 'FK_PROYECTO_TIENE_LOCALIDAD')
alter table LOCALIDADPROYECTO
   drop constraint FK_PROYECTO_TIENE_LOCALIDAD
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PARCELA') and o.name = 'FK_PROYECTO_TIENE_PARCELAS')
alter table PARCELA
   drop constraint FK_PROYECTO_TIENE_PARCELAS
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PROYECTO') and o.name = 'FK_PROYECTO_TIENE_OBJETIVOINVENTARIO')
alter table PROYECTO
   drop constraint FK_PROYECTO_TIENE_OBJETIVOINVENTARIO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PROYECTO') and o.name = 'FK_USUARIO_CREA_PROYECTO')
alter table PROYECTO
   drop constraint FK_USUARIO_CREA_PROYECTO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TIPOLINEAINVENTARIOPROYECTO') and o.name = 'FK_PROYECTO_TIENE_TIPOLINEAINV')
alter table TIPOLINEAINVENTARIOPROYECTO
   drop constraint FK_PROYECTO_TIENE_TIPOLINEAINV
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TIPOLINEAINVENTARIOPROYECTO') and o.name = 'FK_TIPOLINEAINV_SE_APLICA_A_PROYECTO')
alter table TIPOLINEAINVENTARIOPROYECTO
   drop constraint FK_TIPOLINEAINV_SE_APLICA_A_PROYECTO
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TIPOLINEAINVENTARIOVARIABLE') and o.name = 'FK_TIPOLINEAINVVAR_PERTENECE_A_TIPOLINEAINV')
alter table TIPOLINEAINVENTARIOVARIABLE
   drop constraint FK_TIPOLINEAINVVAR_PERTENECE_A_TIPOLINEAINV
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TIPOLINEAINVENTARIOVARIABLE') and o.name = 'FK_TIPOLINEAINVVAR_TIENE_VARIABLE')
alter table TIPOLINEAINVENTARIOVARIABLE
   drop constraint FK_TIPOLINEAINVVAR_TIENE_VARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BLOQUE')
            and   type = 'U')
   drop table BLOQUE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('CONFIGURACIONUNIDADMUESTRAL')
            and   name  = 'NROCONFIGURACIONUNIDADMUESTRAL'
            and   indid > 0
            and   indid < 255)
   drop index CONFIGURACIONUNIDADMUESTRAL.NROCONFIGURACIONUNIDADMUESTRAL
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CONFIGURACIONUNIDADMUESTRAL')
            and   type = 'U')
   drop table CONFIGURACIONUNIDADMUESTRAL
go

if exists (select 1
            from  sysobjects
           where  id = object_id('COSTO')
            and   type = 'U')
   drop table COSTO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('DOMINIO')
            and   name  = 'NRODOMINIO'
            and   indid > 0
            and   indid < 255)
   drop index DOMINIO.NRODOMINIO
go

if exists (select 1
            from  sysobjects
           where  id = object_id('DOMINIO')
            and   type = 'U')
   drop table DOMINIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('DOMINIOVARIABLE')
            and   name  = 'FK_DOMINIOVARIABLE'
            and   indid > 0
            and   indid < 255)
   drop index DOMINIOVARIABLE.FK_DOMINIOVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('DOMINIOVARIABLE')
            and   type = 'U')
   drop table DOMINIOVARIABLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ESPECIE')
            and   name  = 'PERTENECE_FK'
            and   indid > 0
            and   indid < 255)
   drop index ESPECIE.PERTENECE_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ESPECIE')
            and   type = 'U')
   drop table ESPECIE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ESTRATO')
            and   type = 'U')
   drop table ESTRATO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FORMULA')
            and   name  = 'NROFORMULA'
            and   indid > 0
            and   indid < 255)
   drop index FORMULA.NROFORMULA
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FORMULA')
            and   type = 'U')
   drop table FORMULA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FORMULAPROYECTO')
            and   name  = 'FK_FORMULAPROYECTO'
            and   indid > 0
            and   indid < 255)
   drop index FORMULAPROYECTO.FK_FORMULAPROYECTO
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FORMULAPROYECTO')
            and   type = 'U')
   drop table FORMULAPROYECTO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FORMULARIO')
            and   name  = 'ES_REPONSABLE_FK'
            and   indid > 0
            and   indid < 255)
   drop index FORMULARIO.ES_REPONSABLE_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FORMULARIO')
            and   name  = 'MANEJA_FK'
            and   indid > 0
            and   indid < 255)
   drop index FORMULARIO.MANEJA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FORMULARIO')
            and   type = 'U')
   drop table FORMULARIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('FORMULAVARIABLE')
            and   name  = 'FK_FORMULAVARIABLE'
            and   indid > 0
            and   indid < 255)
   drop index FORMULAVARIABLE.FK_FORMULAVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FORMULAVARIABLE')
            and   type = 'U')
   drop table FORMULAVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('GRUPOCOMERCIAL')
            and   type = 'U')
   drop table GRUPOCOMERCIAL
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('IMAGEN')
            and   name  = 'IMAGENID'
            and   indid > 0
            and   indid < 255)
   drop index IMAGEN.IMAGENID
go

if exists (select 1
            from  sysobjects
           where  id = object_id('IMAGEN')
            and   type = 'U')
   drop table IMAGEN
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('LINEAINVENTARIO')
            and   name  = 'LLENA_FK'
            and   indid > 0
            and   indid < 255)
   drop index LINEAINVENTARIO.LLENA_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LINEAINVENTARIO')
            and   type = 'U')
   drop table LINEAINVENTARIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('LINEAINVENTARIOESPECIE')
            and   name  = 'FK_LINEAINVENTARIOESPECIE'
            and   indid > 0
            and   indid < 255)
   drop index LINEAINVENTARIOESPECIE.FK_LINEAINVENTARIOESPECIE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LINEAINVENTARIOESPECIE')
            and   type = 'U')
   drop table LINEAINVENTARIOESPECIE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('LINEAINVENTARIOVARIABLE')
            and   name  = 'FK_LINEAINVENTARIOVARIABLE'
            and   indid > 0
            and   indid < 255)
   drop index LINEAINVENTARIOVARIABLE.FK_LINEAINVENTARIOVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LINEAINVENTARIOVARIABLE')
            and   type = 'U')
   drop table LINEAINVENTARIOVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LISTADODECOSTOS')
            and   type = 'U')
   drop table LISTADODECOSTOS
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LISTADODEESPECIES')
            and   type = 'U')
   drop table LISTADODEESPECIES
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LISTADODEESTRATOS')
            and   type = 'U')
   drop table LISTADODEESTRATOS
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LOCALIDAD')
            and   type = 'U')
   drop table LOCALIDAD
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LOCALIDADPROYECTO')
            and   type = 'U')
   drop table LOCALIDADPROYECTO
go

if exists (select 1
            from  sysobjects
           where  id = object_id('OBJETIVOINVENTARIO')
            and   type = 'U')
   drop table OBJETIVOINVENTARIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PARCELA')
            and   name  = 'FK_PARCELA'
            and   indid > 0
            and   indid < 255)
   drop index PARCELA.FK_PARCELA
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PARCELA')
            and   type = 'U')
   drop table PARCELA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PROYECTO')
            and   name  = 'CREA_FK'
            and   indid > 0
            and   indid < 255)
   drop index PROYECTO.CREA_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('PROYECTO')
            and   name  = 'ES_FK'
            and   indid > 0
            and   indid < 255)
   drop index PROYECTO.ES_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PROYECTO')
            and   type = 'U')
   drop table PROYECTO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TIPOFORMULA')
            and   name  = 'NROTIPOFORMULA'
            and   indid > 0
            and   indid < 255)
   drop index TIPOFORMULA.NROTIPOFORMULA
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TIPOFORMULA')
            and   type = 'U')
   drop table TIPOFORMULA
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TIPOLINEAINVENTARIO')
            and   name  = 'NROTIPOLINEINV'
            and   indid > 0
            and   indid < 255)
   drop index TIPOLINEAINVENTARIO.NROTIPOLINEINV
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TIPOLINEAINVENTARIO')
            and   type = 'U')
   drop table TIPOLINEAINVENTARIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TIPOLINEAINVENTARIOPROYECTO')
            and   name  = 'FK_TIPOLINEAINVENTARIOPROYECTO'
            and   indid > 0
            and   indid < 255)
   drop index TIPOLINEAINVENTARIOPROYECTO.FK_TIPOLINEAINVENTARIOPROYECTO
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TIPOLINEAINVENTARIOPROYECTO')
            and   type = 'U')
   drop table TIPOLINEAINVENTARIOPROYECTO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('TIPOLINEAINVENTARIOVARIABLE')
            and   name  = 'FK_TIPOLINEAINVENTARIOVARIABLE'
            and   indid > 0
            and   indid < 255)
   drop index TIPOLINEAINVENTARIOVARIABLE.FK_TIPOLINEAINVENTARIOVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TIPOLINEAINVENTARIOVARIABLE')
            and   type = 'U')
   drop table TIPOLINEAINVENTARIOVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TSTUDENT')
            and   type = 'U')
   drop table TSTUDENT
go

if exists (select 1
            from  sysobjects
           where  id = object_id('USUARIO')
            and   type = 'U')
   drop table USUARIO
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('VARIABLE')
            and   name  = 'NROVARIABLE'
            and   indid > 0
            and   indid < 255)
   drop index VARIABLE.NROVARIABLE
go

if exists (select 1
            from  sysobjects
           where  id = object_id('VARIABLE')
            and   type = 'U')
   drop table VARIABLE
go

if exists(select 1 from systypes where name='SINO')
   execute sp_unbindrule SINO
go

if exists(select 1 from systypes where name='SINO')
   drop type SINO
go

if exists(select 1 from systypes where name='TIPOCOSTO')
   execute sp_unbindrule TIPOCOSTO
go

if exists(select 1 from systypes where name='TIPOCOSTO')
   drop type TIPOCOSTO
go

if exists(select 1 from systypes where name='TIPODISENIOMUESTRAL')
   execute sp_unbindrule TIPODISENIOMUESTRAL
go

if exists(select 1 from systypes where name='TIPODISENIOMUESTRAL')
   drop type TIPODISENIOMUESTRAL
go

if exists(select 1 from systypes where name='TIPODOMINIOVARIABLE')
   execute sp_unbindrule TIPODOMINIOVARIABLE
go

if exists(select 1 from systypes where name='TIPODOMINIOVARIABLE')
   drop type TIPODOMINIOVARIABLE
go

if exists(select 1 from systypes where name='TIPOFORMA')
   execute sp_unbindrule TIPOFORMA
go

if exists(select 1 from systypes where name='TIPOFORMA')
   drop type TIPOFORMA
go

if exists(select 1 from systypes where name='TIPOGRUPOECOLOGICO')
   execute sp_unbindrule TIPOGRUPOECOLOGICO
go

if exists(select 1 from systypes where name='TIPOGRUPOECOLOGICO')
   drop type TIPOGRUPOECOLOGICO
go

if exists(select 1 from systypes where name='TIPOPLAZO')
   execute sp_unbindrule TIPOPLAZO
go

if exists(select 1 from systypes where name='TIPOPLAZO')
   drop type TIPOPLAZO
go

if exists(select 1 from systypes where name='TIPOPROYECTO')
   execute sp_unbindrule TIPOPROYECTO
go

if exists(select 1 from systypes where name='TIPOPROYECTO')
   drop type TIPOPROYECTO
go

if exists(select 1 from systypes where name='TIPOUSUARIO')
   execute sp_unbindrule TIPOUSUARIO
go

if exists(select 1 from systypes where name='TIPOUSUARIO')
   drop type TIPOUSUARIO
go

if exists(select 1 from systypes where name='TIPOVARIABLE')
   execute sp_unbindrule TIPOVARIABLE
go

if exists(select 1 from systypes where name='TIPOVARIABLE')
   drop type TIPOVARIABLE
go

if exists (select 1 from sysobjects where id=object_id('R_SINO') and type='R')
   drop rule  R_SINO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOCOSTO') and type='R')
   drop rule  R_TIPOCOSTO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPODISENIOMUESTRAL') and type='R')
   drop rule  R_TIPODISENIOMUESTRAL
go

if exists (select 1 from sysobjects where id=object_id('R_TIPODOMINIOVARIABLE') and type='R')
   drop rule  R_TIPODOMINIOVARIABLE
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOFORMA') and type='R')
   drop rule  R_TIPOFORMA
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOGRUPOECOLOGICO') and type='R')
   drop rule  R_TIPOGRUPOECOLOGICO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOPLAZO') and type='R')
   drop rule  R_TIPOPLAZO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOPROYECTO') and type='R')
   drop rule  R_TIPOPROYECTO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOUSUARIO') and type='R')
   drop rule  R_TIPOUSUARIO
go

if exists (select 1 from sysobjects where id=object_id('R_TIPOVARIABLE') and type='R')
   drop rule  R_TIPOVARIABLE
go

create rule R_SINO as
      @column in ('S','N')
go

create rule R_TIPOCOSTO as
      @column in ('CF','CV')
go

create rule R_TIPODISENIOMUESTRAL as
      @column in ('S','E','B')
go

create rule R_TIPODOMINIOVARIABLE as
      @column in ('N','U','M')
go

create rule R_TIPOFORMA as
      @column in ('CU','RE','CI')
go

create rule R_TIPOGRUPOECOLOGICO as
      @column in ('TL','NT')
go

create rule R_TIPOPLAZO as
      @column in ('H','D','S','M','A')
go

create rule R_TIPOPROYECTO as
      @column in ('CR','CO','IN')
go

create rule R_TIPOUSUARIO as
      @column in ('AD','NA')
go

create rule R_TIPOVARIABLE as
      @column in ('E','S')
go

/*==============================================================*/
/* Domain: SINO                                                 */
/*==============================================================*/
create type SINO
   from varchar(1) not null
go

execute sp_bindrule R_SINO, SINO
go

/*==============================================================*/
/* Domain: TIPOCOSTO                                            */
/*==============================================================*/
create type TIPOCOSTO
   from varchar(2) not null
go

execute sp_bindrule R_TIPOCOSTO, TIPOCOSTO
go

/*==============================================================*/
/* Domain: TIPODISENIOMUESTRAL                                  */
/*==============================================================*/
create type TIPODISENIOMUESTRAL
   from varchar(1) not null
go

execute sp_bindrule R_TIPODISENIOMUESTRAL, TIPODISENIOMUESTRAL
go

/*==============================================================*/
/* Domain: TIPODOMINIOVARIABLE                                  */
/*==============================================================*/
create type TIPODOMINIOVARIABLE
   from varchar(1) not null
go

execute sp_bindrule R_TIPODOMINIOVARIABLE, TIPODOMINIOVARIABLE
go

/*==============================================================*/
/* Domain: TIPOFORMA                                            */
/*==============================================================*/
create type TIPOFORMA
   from varchar(2) not null
go

execute sp_bindrule R_TIPOFORMA, TIPOFORMA
go

/*==============================================================*/
/* Domain: TIPOGRUPOECOLOGICO                                   */
/*==============================================================*/
create type TIPOGRUPOECOLOGICO
   from varchar(2) not null
go

execute sp_bindrule R_TIPOGRUPOECOLOGICO, TIPOGRUPOECOLOGICO
go

/*==============================================================*/
/* Domain: TIPOPLAZO                                            */
/*==============================================================*/
create type TIPOPLAZO
   from varchar(1) not null
go

execute sp_bindrule R_TIPOPLAZO, TIPOPLAZO
go

/*==============================================================*/
/* Domain: TIPOPROYECTO                                         */
/*==============================================================*/
create type TIPOPROYECTO
   from varchar(2) not null
go

execute sp_bindrule R_TIPOPROYECTO, TIPOPROYECTO
go

/*==============================================================*/
/* Domain: TIPOUSUARIO                                          */
/*==============================================================*/
create type TIPOUSUARIO
   from varchar(2) not null
go

execute sp_bindrule R_TIPOUSUARIO, TIPOUSUARIO
go

/*==============================================================*/
/* Domain: TIPOVARIABLE                                         */
/*==============================================================*/
create type TIPOVARIABLE
   from varchar(1) not null
go

execute sp_bindrule R_TIPOVARIABLE, TIPOVARIABLE
go

/*==============================================================*/
/* Table: BLOQUE                                                */
/*==============================================================*/
create table BLOQUE (
   NROPROYCONTENEDOR    uniqueidentifier     not null,
   NROPROYCONTENIDO     uniqueidentifier     not null,
   CODIGOBLOQUE         int                  not null,
   PESO                 double precision     not null,
   ETAPA                int                  not null,
   CONFIGURACIONMAPA    xml                  not null,
   constraint PK_BLOQUE primary key (NROPROYCONTENEDOR, NROPROYCONTENIDO)
)
go

/*==============================================================*/
/* Table: CONFIGURACIONUNIDADMUESTRAL                           */
/*==============================================================*/
create table CONFIGURACIONUNIDADMUESTRAL (
   NROCONFIGURACIONUNIDADMUESTRAL uniqueidentifier     not null,
   NROPROY              uniqueidentifier     not null,
   ANCHO                double precision     not null,
   LARGO                double precision     not null,
   RADIO                double precision     not null,
   FORMA                TIPOFORMA            not null,
   TIPOUNIDADMUESTRAL   varchar(1)           not null
      constraint CKC_TIPOUNIDADMUESTRA_CONFIGUR check (TIPOUNIDADMUESTRAL in ('P','B')),
   constraint PK_CONFIGURACIONUNIDADMUESTRAL primary key (NROCONFIGURACIONUNIDADMUESTRAL)
)
go

/*==============================================================*/
/* Index: NROCONFIGURACIONUNIDADMUESTRAL                        */
/*==============================================================*/
create index NROCONFIGURACIONUNIDADMUESTRAL on CONFIGURACIONUNIDADMUESTRAL (
NROCONFIGURACIONUNIDADMUESTRAL ASC
)
go

/*==============================================================*/
/* Table: COSTO                                                 */
/*==============================================================*/
create table COSTO (
   NROCOSTO             uniqueidentifier     not null,
   NOMBRE               varchar(150)         null,
   DESCRIPCION          varchar(500)         null,
   TIPO                 varchar(2)           null,
   constraint PK_COSTO primary key nonclustered (NROCOSTO)
)
go

/*==============================================================*/
/* Table: DOMINIO                                               */
/*==============================================================*/
create table DOMINIO (
   NRODOMINIO           uniqueidentifier     not null,
   NOMBRE               varchar(150)         not null,
   DESCRIPCION          varchar(5000)        null,
   constraint PK_DOMINIO primary key nonclustered (NRODOMINIO)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Restringe los valores que una variable de tipo descriptivo puede tener asociados, ejemplo: Variable CALIDAD, dominio: Bueno o malo.',
   'user', @CurrentUser, 'table', 'DOMINIO'
go

/*==============================================================*/
/* Index: NRODOMINIO                                            */
/*==============================================================*/
create index NRODOMINIO on DOMINIO (
NRODOMINIO ASC
)
go

/*==============================================================*/
/* Table: DOMINIOVARIABLE                                       */
/*==============================================================*/
create table DOMINIOVARIABLE (
   NROVARIABLE          uniqueidentifier     not null,
   NRODOMINIO           uniqueidentifier     not null,
   constraint PK_DOMINIOVARIABLE primary key (NROVARIABLE, NRODOMINIO)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla variable',
   'user', @CurrentUser, 'table', 'DOMINIOVARIABLE', 'column', 'NROVARIABLE'
go

/*==============================================================*/
/* Index: FK_DOMINIOVARIABLE                                    */
/*==============================================================*/
create index FK_DOMINIOVARIABLE on DOMINIOVARIABLE (
NROVARIABLE ASC,
NRODOMINIO ASC
)
go

/*==============================================================*/
/* Table: ESPECIE                                               */
/*==============================================================*/
create table ESPECIE (
   CODESP               uniqueidentifier     not null,
   GRUPOCOM             char(2)              not null,
   NOMCOMUN             varchar(200)         not null,
   NOMCIENTIFICO        varchar(200)         null,
   FAMILIA              varchar(200)         null,
   ZONAGEOGRAFICA       varchar(200)         null,
   ZONADEVIDA           varchar(200)         null,
   DIAMMINCORTE         double precision     null,
   GRUPOECOLOGICO       TIPOGRUPOECOLOGICO   not null,
   constraint PK_ESPECIE primary key nonclustered (CODESP)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Individuos objeto de estudio que son carcaterizados por el inventario forestal para el reconocimiento de las propiedades de una determinada zona forestal. ',
   'user', @CurrentUser, 'table', 'ESPECIE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave primaria de la table especie. Identifica de forma unica una especie creada para ser caracterizada en el inventario',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'CODESP'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave foranea que especifica el valor comercial dado por el usuario a la especie creada',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'GRUPOCOM'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Nombre utilizada en el lenguaje coloquial para identificar una especie',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'NOMCOMUN'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Nombre utilizado en el lenguaje cientifico para identificar una especie',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'NOMCIENTIFICO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Termino empleado en taxonimia para grupar un conjunto de especies que guardan una relacion entre si',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'FAMILIA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Region',
   'user', @CurrentUser, 'table', 'ESPECIE', 'column', 'ZONAGEOGRAFICA'
go

/*==============================================================*/
/* Index: PERTENECE_FK                                          */
/*==============================================================*/
create index PERTENECE_FK on ESPECIE (
GRUPOCOM ASC
)
go

/*==============================================================*/
/* Table: ESTRATO                                               */
/*==============================================================*/
create table ESTRATO (
   CODEST               int                  identity,
   DESCRIPESTRATO       varchar(500)         null,
   constraint PK_ESTRATO primary key nonclustered (CODEST)
)
go

/*==============================================================*/
/* Table: FORMULA                                               */
/*==============================================================*/
create table FORMULA (
   NROFORMULA           uniqueidentifier     not null,
   NROTIPOFORMULA       uniqueidentifier     not null,
   EXPRESION            varchar(5000)        not null,
   NOMBRE               varchar(150)         not null,
   DESCRIPCION          varchar(5000)        null,
   constraint PK_FORMULA primary key (NROFORMULA)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Establece las formulas que se pueden asociar a un proyecto para el ingreso de expresiones personalizables por el usuario. Esta podran estar disponibles para su posterior uso en la elaboracion de uno o mas proyectos. las formulas continen uno o mas variables o parametros que se encuentran almacenadas en la relacion formula variable y se utilizaran para evaluar la expresion que esta contiene y de esta forma obtener el valor de la variable de salida asociada a la formula.',
   'user', @CurrentUser, 'table', 'FORMULA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla formulario',
   'user', @CurrentUser, 'table', 'FORMULA', 'column', 'NROFORMULA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Identifica el proposito con que se creo esta formula, y permitira filtrar formulas al momento de ser seleccionas por el usuario',
   'user', @CurrentUser, 'table', 'FORMULA', 'column', 'NROTIPOFORMULA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Expresion matematica asociada a la formula. Para su evaluacion se emplea el evaluador de expresiones AleParse http://www.aleprojects.com/en/doc/parser',
   'user', @CurrentUser, 'table', 'FORMULA', 'column', 'EXPRESION'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Nombre descriptivo de la formula, que permite al usuario una mejor compresion a traves de su nombre del proposito con el que se creo',
   'user', @CurrentUser, 'table', 'FORMULA', 'column', 'NOMBRE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Campo opcional que brinda un mayor grado de detalle del proposito de la formula',
   'user', @CurrentUser, 'table', 'FORMULA', 'column', 'DESCRIPCION'
go

/*==============================================================*/
/* Index: NROFORMULA                                            */
/*==============================================================*/
create index NROFORMULA on FORMULA (
NROFORMULA ASC
)
go

/*==============================================================*/
/* Table: FORMULAPROYECTO                                       */
/*==============================================================*/
create table FORMULAPROYECTO (
   NROPROY              uniqueidentifier     not null,
   NROFORMULA           uniqueidentifier     not null,
   constraint PK_FORMULAPROYECTO primary key (NROPROY, NROFORMULA)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla formulario',
   'user', @CurrentUser, 'table', 'FORMULAPROYECTO', 'column', 'NROFORMULA'
go

/*==============================================================*/
/* Index: FK_FORMULAPROYECTO                                    */
/*==============================================================*/
create index FK_FORMULAPROYECTO on FORMULAPROYECTO (
NROPROY ASC,
NROFORMULA ASC
)
go

/*==============================================================*/
/* Table: FORMULARIO                                            */
/*==============================================================*/
create table FORMULARIO (
   NROFORMULARIO        uniqueidentifier     not null,
   CODEST               int                  not null,
   NROUSUARIO           uniqueidentifier     not null,
   NROPROY              uniqueidentifier     not null,
   CODIGOPARCELA        int                  not null,
   FECHACREACION        datetime             not null,
   HORAINICIO           datetime             null,
   HORAFINAL            datetime             null,
   constraint PK_FORMULARIO primary key nonclustered (NROFORMULARIO)
)
go

/*==============================================================*/
/* Index: MANEJA_FK                                             */
/*==============================================================*/
create index MANEJA_FK on FORMULARIO (
CODEST ASC
)
go

/*==============================================================*/
/* Index: ES_REPONSABLE_FK                                      */
/*==============================================================*/
create index ES_REPONSABLE_FK on FORMULARIO (
NROUSUARIO ASC
)
go

/*==============================================================*/
/* Table: FORMULAVARIABLE                                       */
/*==============================================================*/
create table FORMULAVARIABLE (
   NROVARIABLE          uniqueidentifier     not null,
   NROFORMULA           uniqueidentifier     not null,
   TIPOVARIABLE         TIPOVARIABLE         not null,
   constraint PK_FORMULAVARIABLE primary key (NROVARIABLE, NROFORMULA)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Parametros de entrada para la formula, contiene la llave foranea de variable y formula. Estos valores se emplean para evaluar la expresion asociada a la formula',
   'user', @CurrentUser, 'table', 'FORMULAVARIABLE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave foranea que identifica la variable asociada a la formula',
   'user', @CurrentUser, 'table', 'FORMULAVARIABLE', 'column', 'NROVARIABLE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave foranea en la tabla FormulaVariable. Identifica la formula que se asocia a las variables de entrada en la tabla FormulaVariable',
   'user', @CurrentUser, 'table', 'FORMULAVARIABLE', 'column', 'NROFORMULA'
go

/*==============================================================*/
/* Index: FK_FORMULAVARIABLE                                    */
/*==============================================================*/
create index FK_FORMULAVARIABLE on FORMULAVARIABLE (
NROVARIABLE ASC,
NROFORMULA ASC
)
go

/*==============================================================*/
/* Table: GRUPOCOMERCIAL                                        */
/*==============================================================*/
create table GRUPOCOMERCIAL (
   GRUPOCOM             char(2)              not null,
   DESCRIPGRUPO         varchar(500)         null,
   constraint PK_GRUPOCOMERCIAL primary key nonclustered (GRUPOCOM)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Atributo que permite establecer el valor comercial que puede tener asociado una especie.',
   'user', @CurrentUser, 'table', 'GRUPOCOMERCIAL'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Nombre de facil reconocimiento dado por el usuario para carcaterizar una especie comercialmente.',
   'user', @CurrentUser, 'table', 'GRUPOCOMERCIAL', 'column', 'GRUPOCOM'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Descripcion opcional dada por el usuario para un mayor entendimiento de la caracterizacion comercial dada a una especie',
   'user', @CurrentUser, 'table', 'GRUPOCOMERCIAL', 'column', 'DESCRIPGRUPO'
go

/*==============================================================*/
/* Table: IMAGEN                                                */
/*==============================================================*/
create table IMAGEN (
   NROIMAGEN            uniqueidentifier     not null,
   CODESP               uniqueidentifier     not null,
   DESCRIPCION          varchar(1000)        null,
   NOMBRE               varchar(250)         not null,
   RUTA                 varchar(1500)        not null,
   constraint PK_IMAGEN primary key (NROIMAGEN, CODESP)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Asocia un conjunto de imagenes clave, como hojas, tallos, raices, etc. Para la facil identificacion de especies.',
   'user', @CurrentUser, 'table', 'IMAGEN'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave primaria en la table imagen. Identifica de forma unica una imagen asocia a una especie',
   'user', @CurrentUser, 'table', 'IMAGEN', 'column', 'NROIMAGEN'
go

/*==============================================================*/
/* Index: IMAGENID                                              */
/*==============================================================*/
create index IMAGENID on IMAGEN (
NROIMAGEN ASC
)
go

/*==============================================================*/
/* Table: LINEAINVENTARIO                                       */
/*==============================================================*/
create table LINEAINVENTARIO (
   NROLINEAINVENTARIO   uniqueidentifier     not null,
   NROFORMULARIO        uniqueidentifier     not null,
   NROTIPOLINEAINV      uniqueidentifier     not null,
   constraint PK_LINEAINVENTARIO primary key nonclustered (NROLINEAINVENTARIO)
)
go

/*==============================================================*/
/* Index: LLENA_FK                                              */
/*==============================================================*/
create index LLENA_FK on LINEAINVENTARIO (
NROFORMULARIO ASC
)
go

/*==============================================================*/
/* Table: LINEAINVENTARIOESPECIE                                */
/*==============================================================*/
create table LINEAINVENTARIOESPECIE (
   CODESP               uniqueidentifier     not null,
   NROLINEAINVENTARIO   uniqueidentifier     not null,
   constraint PK_LINEAINVENTARIOESPECIE primary key (CODESP, NROLINEAINVENTARIO)
)
go

/*==============================================================*/
/* Index: FK_LINEAINVENTARIOESPECIE                             */
/*==============================================================*/
create index FK_LINEAINVENTARIOESPECIE on LINEAINVENTARIOESPECIE (
CODESP ASC,
NROLINEAINVENTARIO ASC
)
go

/*==============================================================*/
/* Table: LINEAINVENTARIOVARIABLE                               */
/*==============================================================*/
create table LINEAINVENTARIOVARIABLE (
   NROVARIABLE          uniqueidentifier     not null,
   LINEAINV             uniqueidentifier     not null,
   VALOR                varchar(5000)        not null,
   constraint PK_LINEAINVENTARIOVARIABLE primary key (NROVARIABLE, LINEAINV)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla variable',
   'user', @CurrentUser, 'table', 'LINEAINVENTARIOVARIABLE', 'column', 'NROVARIABLE'
go

/*==============================================================*/
/* Index: FK_LINEAINVENTARIOVARIABLE                            */
/*==============================================================*/
create index FK_LINEAINVENTARIOVARIABLE on LINEAINVENTARIOVARIABLE (
NROVARIABLE ASC,
LINEAINV ASC
)
go

/*==============================================================*/
/* Table: LISTADODECOSTOS                                       */
/*==============================================================*/
create table LISTADODECOSTOS (
   NROCOSTO             uniqueidentifier     not null,
   NROPROY              uniqueidentifier     not null,
   VALOR                double precision     not null,
   constraint PK_LISTADODECOSTOS primary key (NROCOSTO, NROPROY)
)
go

/*==============================================================*/
/* Table: LISTADODEESPECIES                                     */
/*==============================================================*/
create table LISTADODEESPECIES (
   CODESP               uniqueidentifier     not null,
   NROPROY              uniqueidentifier     not null,
   constraint PK_LISTADODEESPECIES primary key (CODESP, NROPROY)
)
go

/*==============================================================*/
/* Table: LISTADODEESTRATOS                                     */
/*==============================================================*/
create table LISTADODEESTRATOS (
   NROPROY              uniqueidentifier     not null,
   CODEST               int                  not null,
   PESO                 double precision     null,
   TAMANIOMUESTRA       double precision     null,
   CONFIGURACIONMAPA    xml                  not null,
   constraint PK_LISTADODEESTRATOS primary key (NROPROY, CODEST)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Cuando se indica que el proyecto corresponde a un tipo de diseño estratificado es necesario, relacionar los estratos que componen el proyecto',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Llave foranea que indica cual es el proyecto estratificado',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS', 'column', 'NROPROY'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Estrato asociado al proyecto',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS', 'column', 'CODEST'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Cada estrato existe en una proporcion diferente a cada estrato que compone el proyecto',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS', 'column', 'PESO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Dentro de cada estrato es posible tomar una tamaño de muestral igual o diferente entre cada uno de los estratos en los que se estratifico el proyecto.',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS', 'column', 'TAMANIOMUESTRA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Proporciona informacion de como ubicar cada estrato dentro del proyecto',
   'user', @CurrentUser, 'table', 'LISTADODEESTRATOS', 'column', 'CONFIGURACIONMAPA'
go

/*==============================================================*/
/* Table: LOCALIDAD                                             */
/*==============================================================*/
create table LOCALIDAD (
   CODLOCALIDAD         int                  not null,
   CODLOCALIDADPADRE    int                  null,
   NOMBRE               varchar(1500)        not null,
   constraint PK_LOCALIDAD primary key (CODLOCALIDAD)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Identifica el sitioo los sitios que estan involucrados en el desarrollo del proyecto',
   'user', @CurrentUser, 'table', 'LOCALIDAD'
go

/*==============================================================*/
/* Table: LOCALIDADPROYECTO                                     */
/*==============================================================*/
create table LOCALIDADPROYECTO (
   NROPROY              uniqueidentifier     not null,
   CODLOCALIDAD         int                  not null,
   constraint PK_LOCALIDADPROYECTO primary key (NROPROY, CODLOCALIDAD)
)
go

/*==============================================================*/
/* Table: OBJETIVOINVENTARIO                                    */
/*==============================================================*/
create table OBJETIVOINVENTARIO (
   NOMBRETIPOINV        varchar(100)         not null,
   DESCRIPOBJETINV      varchar(500)         null,
   constraint PK_OBJETIVOINVENTARIO primary key nonclustered (NOMBRETIPOINV)
)
go

/*==============================================================*/
/* Table: PARCELA                                               */
/*==============================================================*/
create table PARCELA (
   NROPROY              uniqueidentifier     not null,
   CODIGOPARCELA        int                  not null,
   LINEAPARCELA         int                  not null,
   CONFIGURACIONMAPA    xml                  not null,
   constraint PK_PARCELA primary key (NROPROY, CODIGOPARCELA)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'La parcela corresponde a las unidades muestrales empleadas para la recolecion de informacion de una determinada area del bosque en estudio. Las parcelas son identificadas acorde a una linea y codigo de parcela adiganado por quien realiza el diseño muestral y relaciona uno o mas formulacion con cada uno de las lineas de inventario recolectadas',
   'user', @CurrentUser, 'table', 'PARCELA'
go

/*==============================================================*/
/* Index: FK_PARCELA                                            */
/*==============================================================*/
create index FK_PARCELA on PARCELA (
NROPROY ASC,
CODIGOPARCELA ASC
)
go

/*==============================================================*/
/* Table: PROYECTO                                              */
/*==============================================================*/
create table PROYECTO (
   NROPROY              uniqueidentifier     not null,
   NROUSUARIO           uniqueidentifier     not null,
   NOMBRETIPOINV        varchar(100)         not null,
   FECHA                datetime             not null,
   NOMBRE               varchar(150)         not null,
   DETALLELUGAR         varchar(1500)        not null,
   DESCRIPCION          varchar(5000)        null,
   PRESUPUESTO          double precision     not null,
   PLAZO                double precision     not null,
   TIPOPLAZO            TIPOPLAZO            not null,
   TIPOPROYECTO         TIPOPROYECTO         not null,
   NUMEROPERSONAS       int                  not null,
   TIPODISENIOMUESTRAL  TIPODISENIOMUESTRAL  not null,
   SELECCIONALEATORIA   SINO                 not null,
   CONFIANZA            double precision     not null,
   NUMEROPARCELAS       int                  not null,
   NUMEROPARCELASAMUESTREAR int                  not null,
   TAMANIOPARCELA       double precision     not null,
   AREATOTAL            double precision     not null,
   INTMUESTREO          double precision     not null,
   AREAMUESTREAR        double precision     not null,
   LIMITINFDAP          double precision     not null,
   AREAREGENERACION     double precision     not null,
   FACTORDEFORMA        double precision     not null,
   ETAPA                int                  not null,
   CONFIGURACIONMAPA    xml                  null,
   constraint PK_PROYECTO primary key nonclustered (NROPROY)
)
go

/*==============================================================*/
/* Index: ES_FK                                                 */
/*==============================================================*/
create index ES_FK on PROYECTO (
NOMBRETIPOINV ASC
)
go

/*==============================================================*/
/* Index: CREA_FK                                               */
/*==============================================================*/
create index CREA_FK on PROYECTO (
NROUSUARIO ASC
)
go

/*==============================================================*/
/* Table: TIPOFORMULA                                           */
/*==============================================================*/
create table TIPOFORMULA (
   NROTIPOFORMULA       uniqueidentifier     not null,
   NOMBRETIPO           varchar(150)         not null,
   DESCRIPCION          varchar(5000)        null,
   constraint PK_TIPOFORMULA primary key (NROTIPOFORMULA)
)
go

/*==============================================================*/
/* Index: NROTIPOFORMULA                                        */
/*==============================================================*/
create index NROTIPOFORMULA on TIPOFORMULA (
NROTIPOFORMULA ASC
)
go

/*==============================================================*/
/* Table: TIPOLINEAINVENTARIO                                   */
/*==============================================================*/
create table TIPOLINEAINVENTARIO (
   NROTIPOLINEAINV      uniqueidentifier     not null,
   NOMBRE               varchar(150)         not null,
   DESCRIPCION          varchar(5000)        null,
   REQUIEREESPECIE      SINO                 not null,
   constraint PK_TIPOLINEAINVENTARIO primary key nonclustered (NROTIPOLINEAINV)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Relaciona  un grupo de variables al momento de su recoleccion. Las variables agrupadas se encuentran relacionadas con un  proposito dentro de la recolecion de datos en el inventario, por ejemplo Lineas no maderables, agrupa variables relacionadas con especies que identifican un proposito adicional al maderero como alimenticio, medicinal, o industrial. Los tipos de lineas de inventario permiten dar una mejor presentacion a la informacion y de igual modo una facil presentacion al usuario',
   'user', @CurrentUser, 'table', 'TIPOLINEAINVENTARIO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Asocia',
   'user', @CurrentUser, 'table', 'TIPOLINEAINVENTARIO', 'column', 'NOMBRE'
go

/*==============================================================*/
/* Index: NROTIPOLINEINV                                        */
/*==============================================================*/
create index NROTIPOLINEINV on TIPOLINEAINVENTARIO (
NROTIPOLINEAINV ASC
)
go

/*==============================================================*/
/* Table: TIPOLINEAINVENTARIOPROYECTO                           */
/*==============================================================*/
create table TIPOLINEAINVENTARIOPROYECTO (
   NROTIPOLINEAINV      uniqueidentifier     not null,
   NROPROY              uniqueidentifier     not null,
   constraint PK_TIPOLINEAINVENTARIOPROYECTO primary key (NROTIPOLINEAINV, NROPROY)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Asocia uno o mas tipos de lineas de inventario a un proyecto.',
   'user', @CurrentUser, 'table', 'TIPOLINEAINVENTARIOPROYECTO'
go

/*==============================================================*/
/* Index: FK_TIPOLINEAINVENTARIOPROYECTO                        */
/*==============================================================*/
create index FK_TIPOLINEAINVENTARIOPROYECTO on TIPOLINEAINVENTARIOPROYECTO (
NROTIPOLINEAINV ASC,
NROPROY ASC
)
go

/*==============================================================*/
/* Table: TIPOLINEAINVENTARIOVARIABLE                           */
/*==============================================================*/
create table TIPOLINEAINVENTARIOVARIABLE (
   NROTIPOLINEAINV      uniqueidentifier     not null,
   NROVARIABLE          uniqueidentifier     not null,
   constraint PK_TIPOLINEAINVENTARIOVARIABLE primary key (NROTIPOLINEAINV, NROVARIABLE)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla variable',
   'user', @CurrentUser, 'table', 'TIPOLINEAINVENTARIOVARIABLE', 'column', 'NROVARIABLE'
go

/*==============================================================*/
/* Index: FK_TIPOLINEAINVENTARIOVARIABLE                        */
/*==============================================================*/
create index FK_TIPOLINEAINVENTARIOVARIABLE on TIPOLINEAINVENTARIOVARIABLE (
NROTIPOLINEAINV ASC,
NROVARIABLE ASC
)
go

/*==============================================================*/
/* Table: TSTUDENT                                              */
/*==============================================================*/
create table TSTUDENT (
   N                    int                  not null,
   ALPHA                double precision     not null,
   VALOR                double precision     not null,
   constraint PK_TSTUDENT primary key nonclustered (N, ALPHA, VALOR)
)
go

/*==============================================================*/
/* Table: USUARIO                                               */
/*==============================================================*/
create table USUARIO (
   NROUSUARIO           uniqueidentifier     not null,
   NOMBRES              varchar(100)         not null,
   APELLIDOS            varchar(100)         not null,
   NOMBREUSUARIO        varchar(100)         not null,
   CONTRASENA           varchar(1000)        not null,
   CEDULA               int                  not null,
   TIPOUSUARIO          TIPOUSUARIO          not null,
   constraint PK_USUARIO primary key nonclustered (NROUSUARIO)
)
go

/*==============================================================*/
/* Table: VARIABLE                                              */
/*==============================================================*/
create table VARIABLE (
   NROVARIABLE          uniqueidentifier     not null,
   ABREVIATURA          varchar(10)          not null,
   NOMBRE               varchar(100)         not null,
   DESCRIPCION          varchar(5000)        null,
   VARIABLEDESCRIPTIVA  SINO                 not null,
   TIPODOMINIO          TIPODOMINIOVARIABLE  not null,
   constraint PK_VARIABLE primary key (NROVARIABLE)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Establece las variables involucradas en el desarrollo de una expresion asociada con una formula. Las variables pueden ser descriptivas, o texto y puede estar restrigindas a un dominio, ejemplo: la variable CALIDAD, es descriptiva y esta amarrada a un dominio: BUENO o MALO en el caso que se idinque no tiene multiple dominio. En otros casos la variable puede asumir varios valores por ejemplo TIPO  USO: alimenticio, medicinal y comercial.',
   'user', @CurrentUser, 'table', 'VARIABLE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Clave primaria en la tabla variable',
   'user', @CurrentUser, 'table', 'VARIABLE', 'column', 'NROVARIABLE'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Campo opcional que permite una mayor grado de compresion del proposito de la variable',
   'user', @CurrentUser, 'table', 'VARIABLE', 'column', 'DESCRIPCION'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Indica si la variable es numerica o texto',
   'user', @CurrentUser, 'table', 'VARIABLE', 'column', 'VARIABLEDESCRIPTIVA'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'En el caso que la variable este restringida a un conjunto de valores se inidica que esta manjea un dominio, o no. El dominio puede ser multiple o estar restringido por un conjunto de valores',
   'user', @CurrentUser, 'table', 'VARIABLE', 'column', 'TIPODOMINIO'
go

/*==============================================================*/
/* Index: NROVARIABLE                                           */
/*==============================================================*/
create index NROVARIABLE on VARIABLE (
NROVARIABLE ASC
)
go

alter table BLOQUE
   add constraint FK_BLOQUE_TIENE_PROYECTO foreign key (NROPROYCONTENIDO)
      references PROYECTO (NROPROY)
go

alter table BLOQUE
   add constraint FK_PROYECTO_CONTENEDOR foreign key (NROPROYCONTENEDOR)
      references PROYECTO (NROPROY)
go

alter table CONFIGURACIONUNIDADMUESTRAL
   add constraint FK_PROYECTO_TIENE_CONFIGURACION_UNIDADMUESTRAL foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table DOMINIOVARIABLE
   add constraint FK_DOMINIO_APLICA_VARIABLE foreign key (NRODOMINIO)
      references DOMINIO (NRODOMINIO)
go

alter table DOMINIOVARIABLE
   add constraint FK_VARIABLE_TIENE_DOMINIO foreign key (NROVARIABLE)
      references VARIABLE (NROVARIABLE)
go

alter table ESPECIE
   add constraint FK_ESPECIE_PERTENECE_GRUPOCOMERCIAL foreign key (GRUPOCOM)
      references GRUPOCOMERCIAL (GRUPOCOM)
go

alter table FORMULA
   add constraint FK_FORMULA_ES_DE_TIPO_FORMULA foreign key (NROTIPOFORMULA)
      references TIPOFORMULA (NROTIPOFORMULA)
go

alter table FORMULAPROYECTO
   add constraint FK_FORMULA_SE_APLICA_A_PROYECTOS foreign key (NROFORMULA)
      references FORMULA (NROFORMULA)
go

alter table FORMULAPROYECTO
   add constraint FK_PROYECTO_TIENE_FORMULA foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table FORMULARIO
   add constraint FK_FORMULARIO_TIENE_ESTRATO foreign key (CODEST)
      references ESTRATO (CODEST)
go

alter table FORMULARIO
   add constraint FK_PARCELA_TIENE_FORMULARIO foreign key (NROPROY, CODIGOPARCELA)
      references PARCELA (NROPROY, CODIGOPARCELA)
go

alter table FORMULARIO
   add constraint FK_USUARIO_CREA_FOMULARIO foreign key (NROUSUARIO)
      references USUARIO (NROUSUARIO)
go

alter table FORMULAVARIABLE
   add constraint FK_FORMULA_TIENE_VARIABLE foreign key (NROFORMULA)
      references FORMULA (NROFORMULA)
go

alter table FORMULAVARIABLE
   add constraint FK_VARIABLE_SE_APLICA_FORMULA foreign key (NROVARIABLE)
      references VARIABLE (NROVARIABLE)
go

alter table IMAGEN
   add constraint FK_ESPECIE_TIENE_IMAGEN foreign key (CODESP)
      references ESPECIE (CODESP)
         on update cascade on delete cascade
go

alter table LINEAINVENTARIO
   add constraint FK_FORMULARIO_CREA_LINEAINEVNTARIO foreign key (NROFORMULARIO)
      references FORMULARIO (NROFORMULARIO)
go

alter table LINEAINVENTARIO
   add constraint FK_LINEA_INVENTARIO_ES_DE_TIPOLINEAINV foreign key (NROTIPOLINEAINV)
      references TIPOLINEAINVENTARIO (NROTIPOLINEAINV)
go

alter table LINEAINVENTARIOESPECIE
   add constraint FK_ESPECIE_PERTENECE_LINEAINVENTARIO foreign key (CODESP)
      references ESPECIE (CODESP)
go

alter table LINEAINVENTARIOESPECIE
   add constraint FK_LINEA_INVENTARIO_TIENE_ESPECIE foreign key (NROLINEAINVENTARIO)
      references LINEAINVENTARIO (NROLINEAINVENTARIO)
go

alter table LINEAINVENTARIOVARIABLE
   add constraint FK_LINEAINVENTARIO_TIENE_VARIABLES foreign key (NROVARIABLE)
      references VARIABLE (NROVARIABLE)
go

alter table LINEAINVENTARIOVARIABLE
   add constraint FK_VARIABLES_SE_APLICA_LINEAINVENTARIO foreign key (LINEAINV)
      references LINEAINVENTARIO (NROLINEAINVENTARIO)
go

alter table LISTADODECOSTOS
   add constraint FK_LISTADODECOSTOS_TIENE_COSTO foreign key (NROCOSTO)
      references COSTO (NROCOSTO)
go

alter table LISTADODECOSTOS
   add constraint FK_PROYECTO_TIENE_LISTADODECOSTOS foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table LISTADODEESPECIES
   add constraint FK_LISTADODEESPECIES_TIENE_ESPECIE foreign key (CODESP)
      references ESPECIE (CODESP)
go

alter table LISTADODEESPECIES
   add constraint FK_PROYECTO_TIENE_LISTADODEESPECIE foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table LISTADODEESTRATOS
   add constraint FK_LISTADEESTRATOS_TIENE_ESTRATOS foreign key (CODEST)
      references ESTRATO (CODEST)
go

alter table LISTADODEESTRATOS
   add constraint FK_PROYECTO_TIENE_LISTADODEESTRATOS foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table LOCALIDAD
   add constraint FK_LOCALIDAD_ESTA_CONTENIDA_LOCALIDAD foreign key (CODLOCALIDADPADRE)
      references LOCALIDAD (CODLOCALIDAD)
go

alter table LOCALIDADPROYECTO
   add constraint FK_LOCALIDAD_ESTA_ASOCIADA_PROYECTO foreign key (CODLOCALIDAD)
      references LOCALIDAD (CODLOCALIDAD)
go

alter table LOCALIDADPROYECTO
   add constraint FK_PROYECTO_TIENE_LOCALIDAD foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table PARCELA
   add constraint FK_PROYECTO_TIENE_PARCELAS foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table PROYECTO
   add constraint FK_PROYECTO_TIENE_OBJETIVOINVENTARIO foreign key (NOMBRETIPOINV)
      references OBJETIVOINVENTARIO (NOMBRETIPOINV)
go

alter table PROYECTO
   add constraint FK_USUARIO_CREA_PROYECTO foreign key (NROUSUARIO)
      references USUARIO (NROUSUARIO)
go

alter table TIPOLINEAINVENTARIOPROYECTO
   add constraint FK_PROYECTO_TIENE_TIPOLINEAINV foreign key (NROTIPOLINEAINV)
      references TIPOLINEAINVENTARIO (NROTIPOLINEAINV)
go

alter table TIPOLINEAINVENTARIOPROYECTO
   add constraint FK_TIPOLINEAINV_SE_APLICA_A_PROYECTO foreign key (NROPROY)
      references PROYECTO (NROPROY)
go

alter table TIPOLINEAINVENTARIOVARIABLE
   add constraint FK_TIPOLINEAINVVAR_PERTENECE_A_TIPOLINEAINV foreign key (NROTIPOLINEAINV)
      references TIPOLINEAINVENTARIO (NROTIPOLINEAINV)
go

alter table TIPOLINEAINVENTARIOVARIABLE
   add constraint FK_TIPOLINEAINVVAR_TIENE_VARIABLE foreign key (NROVARIABLE)
      references VARIABLE (NROVARIABLE)
go

