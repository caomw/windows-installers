﻿using System;
using System.Linq;
using Elastic.Installer.Domain.Elasticsearch.Model;
using Elastic.Installer.Domain.Model;
using FluentAssertions;

namespace Elastic.Installer.Domain.Tests.Elasticsearch.Models
{
	public class InstallationModelArgumentsTestsBase : InstallationModelTestBase
	{
		protected void Argument<T>(string key, T value, Action<InstallationModel> assert) =>
			this.Argument(key, value, (s, v) => assert(s));

		protected void Argument<T>(string key, T value, Action<InstallationModel, T> assert)
		{
			var model = WithValidPreflightChecks().InstallationModel;
			string msiParams = AssertParser(model, key, value, assert);
			var msiParamString = model.ParsedArguments.MsiString(value);

			msiParams.Should().Contain($"{key.ToUpperInvariant()}=\"{msiParamString}\"");

		}

		protected void Argument<T>(string key, T value, string msiParam, Action<InstallationModel, T> assert)
		{
			var model = WithValidPreflightChecks().InstallationModel;
			string msiParams = AssertParser(model, key, value, assert);
			msiParams.Should().Contain($"{key.ToUpperInvariant()}=\"{msiParam}\"");

		}

		private string AssertParser<T>(InstallationModel model, string key, T value, Action<InstallationModel, T> assert)
		{
			var args = new[] { $"{key.Split('.').Last()}={value}" };
			var models = model.Steps.Cast<IValidatableReactiveObject>().Concat(new[] { model }).ToList();
			var viewModelArgumentParser = new ModelArgumentParser(models, args);
			assert(model, value);

			var msiParams = model.ToMsiParamsString();
			msiParams.Should().NotBeEmpty();
			return msiParams;
		}
	}
}
