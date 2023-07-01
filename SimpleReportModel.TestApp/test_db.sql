create table parent (
id int not null,
name nvarchar(100) not null,
nickname nvarchar(100) null
)

create table child (
id int not null,
parent_id int not null,
name nvarchar(100) not null,
nickname nvarchar(100) null
)

insert parent 
values (1, 'Me', 'Hardy Blackie'), (2, 'You', null)

insert child
values (1, 1, 'Kid1', null), (2, 2, 'Kid2', 'goldie')
insert child
values (3, 1, 'Kid3', 'ultimo')

select 
  p.id AS 'Parent.Id'
  , p.name AS 'Parent.Name'
  , p.nickname AS 'Parent.NickName'
  , k.id AS 'Kid.Id'
  , k.name AS 'Kid.Name'
  , k.nickname AS 'Kid.NickName'
from parent as p
  left outer join child as k
    on k.parent_id = p.id
FOR JSON PATH

drop view parent_childs
select * from parent_childs

create view parent_childs(json)
as
select 
  p.id AS 'Parent.Id'
  , p.name AS 'Parent.Name'
  , p.nickname AS 'Parent.NickName'
  , (SELECT 
      k.id
      , k.name 
      , k.nickname
    FROM child as k
    WHERE k.parent_id = p.id
    FOR JSON PATH ) as Kids
from parent as p
FOR JSON PATH

