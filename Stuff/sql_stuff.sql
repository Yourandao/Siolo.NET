create or replace function get_hosts_info()
returns table (
	host_mask int,
	host_ip varchar,
	wildcart varchar
) language plpgsql as $$
begin
	return query select entity.mask, entity.ip, p.wildcart from entity
				 left join restriction as r on r.entity_id = entity.id
				 left join policy as p on r.policy_id = p.id;
end;
$$

create or replace function register(_mask int, _ip varchar, _subnet boolean)
returns uuid
language plpgsql as $$
declare
	temp_entity_id uuid;
begin
	temp_entity_id = uuid_nil();

	if not exists (select * from entity where _mask = mask and _ip = ip) then
		insert into entity(id, mask, ip, is_subnet) values (uuid_generate_v4(), _mask, _ip, _subnet) 
			returning id into temp_entity_id;
	end if;
	
	return temp_entity_id;
end;
$$

create or replace function register_policy(_info varchar, _wildcart varchar)
returns uuid language plpgsql as $$
declare
	temp_policy_uuid uuid;
begin
	temp_policy_uuid = uuid_nil();

	if not exists (select id from policy where _wildcart = wildcart) then
		insert into policy (id, info, wildcart)
			values (uuid_generate_v4(), _info, _wildcart)
				returning id into temp_policy_uuid;
				
	end if;
	
	return temp_policy_uuid;
end;
$$

create or replace function register_rule_on_host(_mask int, _ip varchar, _wildcart varchar)
returns bool language plpgsql as $$
declare
	temp_host_uuid uuid;
	temp_policy_uuid uuid;
	
	temp_result uuid;
begin
	if not exists (select id from entity where mask = _mask and _ip = ip) then
		return False;
	else
		select id into temp_host_uuid from entity
			where _mask = mask and _ip = ip;
	end if;
	
	if not exists (select id from policy where wildcart = _wildcart) then
		temp_policy_uuid = register_policy('', _wildcart);
	else
		select into temp_policy_uuid 
			from policy where _wildcart = wildcart;
	end if;
	
	insert into restriction (id, policy_id, entity_id)
		values (uuid_generate_v4(), temp_policy_uuid, temp_host_uuid)
			returning id into temp_result;
			
	return True;
end;
$$



----C# TODO
create or replace function hosts_info()
returns table (
	host_mask int,
	host_ip varchar,
	wildcarts varchar[]
) language plpgsql as $$
declare
	records cursor for select entity.mask, entity.ip, p.wildcart from entity
				 left join restriction as r on r.entity_id = entity.id
				 left join policy as p on r.policy_id = p.id;
begin
	
end;
$$