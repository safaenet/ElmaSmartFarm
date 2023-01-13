﻿using ElmaSmartFarm.DataLibraryCore;
using ElmaSmartFarm.SharedLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace ElmaSmartFarm.Service.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PoultryController : ControllerBase
{
    public PoultryController()
    {

    }

    [HttpGet]
    public async Task<ActionResult<PoultryDtoModel>> GetPoultryAsync()
    {
        var res = await Task.Run<PoultryDtoModel>(() =>
        {
            var poultryEntities = Program.ServiceHost.Services.GetService<PoultryEntities>();
            return new()
            {
                Poultry = poultryEntities.Poultry,
                UnknownMqttMessages = poultryEntities.UnknownMqttMessages,
                AlarmableSensorErrors = poultryEntities.AlarmableSensorErrors,
                AlarmableFarmPeriodErrors = poultryEntities.AlarmableFarmPeriodErrors,
                AlarmablePoultryPeriodErrors = poultryEntities.AlarmablePoultryPeriodErrors,
                SystemUpTime = poultryEntities.SystemStartUpTime
            };
        });
        return res;
    }

    [HttpGet("{FarmNumber}")]
    public async Task<ActionResult<FarmModel>> GetFarmAsync(int FarmNumber)
    {
        var res = await Task.Run(() =>
        {
            var poultryEntities = Program.ServiceHost.Services.GetService<PoultryEntities>();
            return poultryEntities.Poultry.Farms.Where(f => f.FarmNumber == FarmNumber).FirstOrDefault();
        });
        return res;
    }
}