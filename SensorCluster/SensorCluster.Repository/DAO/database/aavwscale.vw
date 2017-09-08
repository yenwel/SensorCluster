create or replace force view aavwscale as
select S.URI, S.LASTVAL, S.TIME, L.LOCLOCID "LOCID"
    from AASENSOR S inner join AALOCGUID L
    on S.LOCGUID = L.ID
   where S.TYPE ='SCALE';

