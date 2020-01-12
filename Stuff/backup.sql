--
-- PostgreSQL database dump
--

-- Dumped from database version 12.1 (Debian 12.1-1.pgdg100+1)
-- Dumped by pg_dump version 12.0

-- Started on 2019-12-14 20:57:10 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';

CREATE FUNCTION public.get_hosts_info() RETURNS TABLE(host_mask integer, host_ip character varying, wildcart character varying)
    LANGUAGE plpgsql
    AS $$begin
	return query select entity.mask, entity.ip, p.wildcart from entity
				 left join restriction as r on r.entity_id = entity.id
				 left join policy as p on r.policy_id = p.id;
end;
$$;

ALTER FUNCTION public.get_hosts_info() OWNER TO postgres;

CREATE FUNCTION public.register(_mask integer, _ip character varying, _subnet boolean) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
declare
	temp_entity_id uuid;
begin
	temp_entity_id = uuid_nil();

	if not exists (select * from entity where _mask = mask and _ip = ip) then
		insert into entity(id, mask, ip, is_subnet) 
			values (uuid_generate_v4(), _mask, _ip, _subnet) 
				returning id into temp_entity_id;
	end if;
	
	return temp_entity_id;
end;
$$;

ALTER FUNCTION public.register(_mask integer, _ip character varying, _subnet boolean) OWNER TO postgres;

CREATE FUNCTION public.register_policy(_info character varying, _wildcart character varying) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
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
$$;


ALTER FUNCTION public.register_policy(_info character varying, _wildcart character varying) OWNER TO postgres;

--
-- TOC entry 230 (class 1255 OID 16494)
-- Name: register_rule_on_host(integer, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.register_rule_on_host(_mask integer, _ip character varying, _wildcart character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
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
$$;

ALTER FUNCTION public.register_rule_on_host(_mask integer, _ip character varying, _wildcart character varying) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

CREATE TABLE public.entity (
    id uuid NOT NULL,
    mask integer NOT NULL,
    ip character varying NOT NULL,
    is_subnet boolean NOT NULL
);

ALTER TABLE public.entity OWNER TO postgres;

CREATE TABLE public.policy (
    id uuid NOT NULL,
    info character varying,
    wildcart character varying NOT NULL
);

ALTER TABLE public.policy OWNER TO postgres;

CREATE TABLE public.restriction (
    id uuid NOT NULL,
    policy_id uuid NOT NULL,
    entity_id uuid NOT NULL
);

ALTER TABLE public.restriction OWNER TO postgres;

ALTER TABLE ONLY public.entity
    ADD CONSTRAINT entity_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.policy
    ADD CONSTRAINT policy_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.restriction
    ADD CONSTRAINT restriction_pkey PRIMARY KEY (id);