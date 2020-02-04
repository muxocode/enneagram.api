# Data
## Creación de BBDD
### DataBase first
Antes de nada seleccionamos el proyecto como proyecto por defecto
#### Command
```shell
PM>Scaffold-DbContext "Data Source=28APO4166\SQL17;Initial Catalog=Enneagram;Integrated Security=True" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```