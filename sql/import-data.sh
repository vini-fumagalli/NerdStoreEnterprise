# Aguarda o SQL Server estar pronto para aceitar conexões
for i in {1..30}; do
    /opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U SA -P "MeuDB@123" -Q "SELECT 1" &> /dev/null
    if [ $? -eq 0 ]; then
        echo "SQL Server está pronto!"
        break
    fi
    echo "Aguardando SQL Server iniciar..."
    sleep 5
done

# Agora executa o script SQL
/opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U SA -P "MeuDB@123" -i criacao-banco-docker.sql