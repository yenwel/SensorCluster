-----------------------------------------------
-- Export file for user LOCUSHAMA@LOCUS      --
-- Created by jvdpitte on 7/06/2016, 9:39:01 --
-----------------------------------------------

set define off
spool sensor.log

prompt
prompt Creating table AALOCGUID
prompt ========================
prompt
@@aalocguid.tab
prompt
prompt Creating table AASENSOR
prompt =======================
prompt
@@aasensor.tab
prompt
prompt Creating view AAVWSCALE
prompt =======================
prompt
@@aavwscale.vw

spool off
