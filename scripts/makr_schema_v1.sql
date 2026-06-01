CREATE TABLE users (
	user_id UUID PRIMARY KEY NOT NULL,
	email VARCHAR(250) NOT NULL UNIQUE, --Use for auth
	password_hash VARCHAR(250),
	email_verified BOOLEAN NOT NULL DEFAULT FALSE,
	last_login_at TIMESTAMPTZ,
	created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE owners (
	owner_id UUID PRIMARY KEY NOT NULL,
	owner_number BIGINT GENERATED ALWAYS AS IDENTITY UNIQUE NOT NULL,
	user_id UUID REFERENCES users(user_id),
	slug VARCHAR(100) UNIQUE NOT NULL,
	bio TEXT,
	created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE templates (
	template_id UUID PRIMARY KEY NOT NULL,
	template_number BIGINT GENERATED ALWAYS AS IDENTITY UNIQUE NOT NULL,
	owner_id UUID REFERENCES owners(owner_id),
	slug VARCHAR(100) NOT NULL,
	visibility VARCHAR(50) NOT NULL,
	current_version_id UUID,
	created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

	CONSTRAINT uq_template_owner_slug UNIQUE (owner_id, slug),

	CONSTRAINT chk_templates_visibility_type
	CHECK (
		visibility IN (
			'PUBLIC',
			'PRIVATE',
			'CUSTOM'
		)
	)
);

CREATE TABLE template_versions (
	version_id UUID PRIMARY KEY NOT NULL,
	version_number BIGINT GENERATED ALWAYS AS IDENTITY UNIQUE NOT NULL,
	template_id UUID REFERENCES templates(template_id),
	name VARCHAR(250) NOT NULL,
	description TEXT,
	ref VARCHAR(250) NOT NULL,
	commit_sha VARCHAR(250) NOT NULL,
	config_json JSONB NOT NULL,
	created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

ALTER TABLE templates
ADD CONSTRAINT fk_template_current_version
FOREIGN KEY (current_version_id)
REFERENCES template_versions(version_id);